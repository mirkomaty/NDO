Public Class UISerializer

#Region " Serialize "

#Region " Public "

    Public Shared Sub Serialize(ByVal item As Object, ByVal fileName As String)
        Dim doc As New XmlDocument
        Serialize(doc, item)
        doc.Save(fileName)
    End Sub

    Public Shared Sub Serialize(ByVal Document As XmlDocument, ByVal Item As Object)
        Serialize(Document, Item, "")
    End Sub

    Public Shared Sub Serialize(ByVal Document As XmlDocument, ByVal Item As Object, ByVal Name As String)
        Dim FirstNode As XmlNode = Document.AppendChild(Document.CreateElement(Item.GetType.FullName))
        If Name <> "" Then
            FirstNode.Attributes.Append(Document.CreateAttribute("Name")).Value = Name
        End If
        SerializeItem(FirstNode, Item, Nothing)
    End Sub

#End Region

#Region " Private "

	Private Shared Sub SerializeItem(ByVal Node As XmlNode, ByVal Parent As Object, ByVal PD As PropertyDescriptor)
		Dim CurrentType As Type
		Dim ChildProperties As PropertyDescriptorCollection
		Dim NewItem As Object
		Dim NewNode As XmlNode

		If PD Is Nothing Then
			CurrentType = Parent.GetType
			ChildProperties = TypeDescriptor.GetProperties(Parent)
		Else
			CurrentType = PD.PropertyType
			ChildProperties = PD.GetChildProperties
		End If

		If IsCollection(CurrentType) Then
			For Each NewItem In DirectCast(Parent, ICollection)
				SerializeItem(Node, NewItem, Nothing)
			Next
		Else
			If Node.Name = CurrentType.FullName Then
				NewNode = Node
			Else
				NewNode = Node.AppendChild(Node.OwnerDocument.CreateElement(GetXmlNodeName(CurrentType.FullName)))
				If Not PD Is Nothing Then
					NewNode.Attributes.Append(Node.OwnerDocument.CreateAttribute("Name")).Value = PD.Name
				End If
			End If

			If CurrentType.FullName = "System.String" OrElse CurrentType.FullName = "System.Drawing.Color" OrElse ChildProperties.Count = 0 Then
				Dim Value As Object = PD.GetValue(Parent)
				Dim NewSubNode As XmlNode

				If CurrentType.FullName = "System.String" AndAlso DirectCast(Value, String) = "" Then
					Node.RemoveChild(NewNode)
					Exit Sub
				End If

				If CurrentType.FullName = "System.Drawing.Color" AndAlso DirectCast(CurrentType.GetProperty("IsEmpty").GetValue(Value, Nothing), Boolean) Then
					Node.RemoveChild(NewNode)
					Exit Sub
				End If

				NewNode.AppendChild(Node.OwnerDocument.CreateTextNode(GetString(Value)))
			Else
				If Not PD Is Nothing Then
					NewItem = PD.GetValue(Parent)
				Else
					NewItem = Parent
				End If

				If Not NewItem Is Nothing Then
					For Each NewPD As PropertyDescriptor In TypeDescriptor.GetProperties(NewItem)
						If (ShouldSerialize(NewItem, NewPD) AndAlso Not IsCollection(NewPD.PropertyType)) OrElse (IsCollection(NewPD.PropertyType) AndAlso Not NewPD.GetValue(NewItem) Is Nothing AndAlso DirectCast(NewPD.GetValue(NewItem), ICollection).Count > 0) Then
							If IsCollection(NewPD.PropertyType) Then
								Dim NewSubNode As XmlNode = NewNode.AppendChild(NewNode.OwnerDocument.CreateElement(GetXmlNodeName(NewPD.PropertyType.FullName)))

								NewSubNode.Attributes.Append(NewNode.OwnerDocument.CreateAttribute("Name")).Value = NewPD.Name

								SerializeItem(NewSubNode, NewPD.GetValue(NewItem), Nothing)
							Else
								SerializeItem(NewNode, NewItem, NewPD)
							End If
						End If
					Next
				Else
					Node.RemoveChild(NewNode)
				End If
			End If
		End If
	End Sub

	Private Shared Function ShouldSerialize(ByVal Parent As Object, ByVal PD As PropertyDescriptor) As Boolean
		Return PD.SerializationVisibility <> DesignerSerializationVisibility.Hidden AndAlso PD.ShouldSerializeValue(Parent)
	End Function

	Private Shared Function IsCollection(ByVal Item As Type) As Boolean
		Return Not Item.GetInterface("ICollection") Is Nothing
	End Function

	Private Shared Function GetXmlNodeName(ByVal FullName As String) As String
		Return FullName.Replace("+", ".")
	End Function

	Private Shared Function GetString(ByVal Value As Object) As String
		Select Case Value.GetType.FullName
			Case "System.Drawing.Color"
				Return Value.GetType.GetMethod("ToArgb").Invoke(Value, Nothing).ToString
			Case Else
				Return Value.ToString
		End Select
	End Function

#End Region

#End Region

#Region " Deserialize "

#Region " Public "

    Public Shared Sub Deserialize(ByVal Parent As Object, ByVal fileName As String)
        Dim doc As New XmlDocument
        doc.Load(fileName)
        Deserialize(Parent, doc)
    End Sub

    Public Shared Sub Deserialize(ByVal Parent As Object, ByVal Node As XmlNode)
        For Each Child As XmlNode In Node.ChildNodes
            DeserializeItem(Parent, Child)
        Next
    End Sub

#End Region

#Region " Private "

	Private Shared Sub DeserializeItem(ByRef Parent As Object, ByVal Node As XmlNode)
		If IsCollection(Parent.GetType) Then
			Dim Temp As Object = GetAssembly(Node.Name.Substring(0, Node.Name.LastIndexOf("."))).CreateInstance(Node.Name)
			CType(Parent, IList).Add(Temp)
			For Each Child As XmlNode In Node.ChildNodes
				DeserializeItem(Temp, Child)
			Next
		Else
			If Node.FirstChild.NodeType = XmlNodeType.Text Then
				Dim PI As PropertyInfo = Parent.GetType().GetProperty(Node.Attributes.GetNamedItem("Name").Value)
				If PI.GetValue(Parent, Nothing) Is Nothing Then
					PI.SetValue(PI, PI.PropertyType.GetConstructor(Nothing).Invoke(Nothing), Nothing)
				End If
				Parent.GetType().GetProperty(Node.Attributes.GetNamedItem("Name").Value).SetValue(Parent, GetValue(Node.Name, Node.FirstChild.Value), Nothing)
			Else
				For Each Child As XmlNode In Node.ChildNodes
					DeserializeItem(Parent.GetType().GetProperty(Node.Attributes.GetNamedItem("Name").Value).GetValue(Parent, Nothing), Child)
				Next
			End If
		End If
	End Sub

	Private Shared Function GetValue(ByVal PropertyType As String, ByVal Value As String) As Object
		Dim BaseLibrary As String = PropertyType.Substring(0, PropertyType.LastIndexOf("."))
		Dim ClassName As String = PropertyType.Substring(PropertyType.LastIndexOf(".") + 1)
		Dim BaseAssembly As [Assembly]

		If BaseLibrary = "System" Then
			'core types
			BaseAssembly = GetAssembly("mscorlib")
			If ClassName = "String" Then
				Return Value
			Else
				Return GetAssembly("mscorlib").GetType(PropertyType, True).GetMethod("Parse", New Type() {GetType(String)}).Invoke(Nothing, New Object() {Value})
			End If
		Else
			'other types
			BaseAssembly = GetAssembly(BaseLibrary)
			Dim CurType As Type = BaseAssembly.GetType(PropertyType, True)
			Dim BaseType As Type = CurType.BaseType

			If BaseType Is GetType([Enum]) Then
				'enums
				Return BaseType.GetMethod("Parse", New Type() {GetType(Type), GetType(String)}).Invoke(Nothing, New Object() {CurType, Value})
			Else
				Select Case PropertyType
					Case "System.Drawing.Color"
						'color
						Return BaseAssembly.GetType(PropertyType, True).GetMethod("FromArgb", New Type() {GetType(Integer)}).Invoke(Nothing, New Object() {Integer.Parse(Value)})
					Case Else
						'everything else
						Throw New Exception("Deserialization for type '" & PropertyType & "' not implemented")
				End Select
			End If
		End If
	End Function

#End Region

#End Region

#Region " Miscellaneous "

	Private Shared Function GetAssembly(ByVal Name As String) As [Assembly]
		Dim Temp As [Assembly]
		For Each Item As [Assembly] In AppDomain.CurrentDomain.GetAssemblies
			If Item.GetName.Name = Name Then
				Temp = Item
				Exit For
			End If
		Next
		Return Temp
	End Function

#End Region

End Class