using System;
using System.Collections;
using NDO;

using NDO.Mapping.Attributes;

namespace RelationTestClasses
{

[NDOPersistent]
public class AgrDir1NoTblAutoLeft
{
	int dummy;
	[NDORelation]
	AgrDir1NoTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1NoTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1NoTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrDir1NoTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrDir1TblAutoLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrDir1TblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1TblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1TblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrDir1TblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrBi11NoTblAutoLeft
{
	int dummy;
	[NDORelation]
	AgrBi11NoTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11NoTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11NoTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi11NoTblAutoRight
{
	int dummy;
	[NDORelation]
	AgrBi11NoTblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11NoTblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi11TblAutoLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11TblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11TblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11TblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi11TblAutoRight
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11TblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11TblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrDirnNoTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(AgrDirnNoTblAutoRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnNoTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrDirnNoTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrDirnTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(AgrDirnTblAutoRight)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrDirnTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrBin1NoTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(AgrBin1NoTblAutoRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1NoTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrBin1NoTblAutoRight
{
	int dummy;
	[NDORelation]
	AgrBin1NoTblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1NoTblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBin1TblAutoLeft
{
	int dummy;
	[NDORelation(typeof(AgrBin1TblAutoRight)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1TblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrBin1TblAutoRight
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBin1TblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1TblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi1nNoTblAutoLeft
{
	int dummy;
	[NDORelation]
	AgrBi1nNoTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nNoTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nNoTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi1nNoTblAutoRight
{
	int dummy;
	[NDORelation(typeof(AgrBi1nNoTblAutoLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi1nTblAutoLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi1nTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi1nTblAutoRight
{
	int dummy;
	[NDORelation(typeof(AgrBi1nTblAutoLeft)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBinnTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(AgrBinnTblAutoRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBinnTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrBinnTblAutoRight
{
	int dummy;
	[NDORelation(typeof(AgrBinnTblAutoLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpDir1NoTblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpDir1NoTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1NoTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1NoTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpDir1NoTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpDir1TblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpDir1TblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1TblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1TblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpDir1TblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpBi11NoTblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi11NoTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11NoTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11NoTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi11NoTblAutoRight
{
	int dummy;
	[NDORelation]
	CmpBi11NoTblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11NoTblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi11TblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi11TblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11TblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11TblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi11TblAutoRight
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBi11TblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11TblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpDirnNoTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(CmpDirnNoTblAutoRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnNoTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpDirnNoTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpDirnTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(CmpDirnTblAutoRight), RelationInfo.Composite), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpDirnTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpBin1NoTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(CmpBin1NoTblAutoRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1NoTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpBin1NoTblAutoRight
{
	int dummy;
	[NDORelation]
	CmpBin1NoTblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1NoTblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBin1TblAutoLeft
{
	int dummy;
	[NDORelation(typeof(CmpBin1TblAutoRight), RelationInfo.Composite), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1TblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpBin1TblAutoRight
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBin1TblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1TblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi1nNoTblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi1nNoTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nNoTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nNoTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi1nNoTblAutoRight
{
	int dummy;
	[NDORelation(typeof(CmpBi1nNoTblAutoLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi1nTblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi1nTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi1nTblAutoRight
{
	int dummy;
	[NDORelation(typeof(CmpBi1nTblAutoLeft)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBinnTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(CmpBinnTblAutoRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBinnTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpBinnTblAutoRight
{
	int dummy;
	[NDORelation(typeof(CmpBinnTblAutoLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrDir1OwnpconNoTblAutoLeftBase
{
	int dummy;
	[NDORelation]
	AgrDir1OwnpconNoTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OwnpconNoTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OwnpconNoTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrDir1OwnpconNoTblAutoLeftDerived : AgrDir1OwnpconNoTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrDir1OwnpconNoTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrDir1OwnpconTblAutoLeftBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrDir1OwnpconTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OwnpconTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OwnpconTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrDir1OwnpconTblAutoLeftDerived : AgrDir1OwnpconTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrDir1OwnpconTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrBi11OwnpconNoTblAutoLeftBase
{
	int dummy;
	[NDORelation]
	AgrBi11OwnpconNoTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpconNoTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OwnpconNoTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi11OwnpconNoTblAutoLeftDerived : AgrBi11OwnpconNoTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrBi11OwnpconNoTblAutoRight
{
	int dummy;
	[NDORelation]
	AgrBi11OwnpconNoTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpconNoTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi11OwnpconTblAutoLeftBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OwnpconTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpconTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OwnpconTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi11OwnpconTblAutoLeftDerived : AgrBi11OwnpconTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrBi11OwnpconTblAutoRight
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OwnpconTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpconTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrDirnOwnpconNoTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrDirnOwnpconNoTblAutoRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnOwnpconNoTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrDirnOwnpconNoTblAutoLeftDerived : AgrDirnOwnpconNoTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrDirnOwnpconNoTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrDirnOwnpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrDirnOwnpconTblAutoRight)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnOwnpconTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrDirnOwnpconTblAutoLeftDerived : AgrDirnOwnpconTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrDirnOwnpconTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrBin1OwnpconNoTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBin1OwnpconNoTblAutoRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1OwnpconNoTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrBin1OwnpconNoTblAutoLeftDerived : AgrBin1OwnpconNoTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrBin1OwnpconNoTblAutoRight
{
	int dummy;
	[NDORelation]
	AgrBin1OwnpconNoTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1OwnpconNoTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBin1OwnpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBin1OwnpconTblAutoRight)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1OwnpconTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrBin1OwnpconTblAutoLeftDerived : AgrBin1OwnpconTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrBin1OwnpconTblAutoRight
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBin1OwnpconTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1OwnpconTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi1nOwnpconTblAutoLeftBase
{
	int dummy;
	[NDORelation]
	AgrBi1nOwnpconTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nOwnpconTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nOwnpconTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi1nOwnpconTblAutoLeftDerived : AgrBi1nOwnpconTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrBi1nOwnpconTblAutoRight
{
	int dummy;
	[NDORelation(typeof(AgrBi1nOwnpconTblAutoLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBinnOwnpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBinnOwnpconTblAutoRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBinnOwnpconTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrBinnOwnpconTblAutoLeftDerived : AgrBinnOwnpconTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrBinnOwnpconTblAutoRight
{
	int dummy;
	[NDORelation(typeof(AgrBinnOwnpconTblAutoLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpDir1OwnpconNoTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpDir1OwnpconNoTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OwnpconNoTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OwnpconNoTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpDir1OwnpconNoTblAutoLeftDerived : CmpDir1OwnpconNoTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpDir1OwnpconNoTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpDir1OwnpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpDir1OwnpconTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OwnpconTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OwnpconTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpDir1OwnpconTblAutoLeftDerived : CmpDir1OwnpconTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpDir1OwnpconTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpBi11OwnpconNoTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi11OwnpconNoTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpconNoTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OwnpconNoTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi11OwnpconNoTblAutoLeftDerived : CmpBi11OwnpconNoTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpBi11OwnpconNoTblAutoRight
{
	int dummy;
	[NDORelation]
	CmpBi11OwnpconNoTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpconNoTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi11OwnpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi11OwnpconTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpconTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OwnpconTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi11OwnpconTblAutoLeftDerived : CmpBi11OwnpconTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpBi11OwnpconTblAutoRight
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBi11OwnpconTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpconTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpDirnOwnpconNoTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpDirnOwnpconNoTblAutoRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnOwnpconNoTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpDirnOwnpconNoTblAutoLeftDerived : CmpDirnOwnpconNoTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpDirnOwnpconNoTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpDirnOwnpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpDirnOwnpconTblAutoRight), RelationInfo.Composite), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnOwnpconTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpDirnOwnpconTblAutoLeftDerived : CmpDirnOwnpconTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpDirnOwnpconTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpBin1OwnpconNoTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBin1OwnpconNoTblAutoRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1OwnpconNoTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpBin1OwnpconNoTblAutoLeftDerived : CmpBin1OwnpconNoTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpBin1OwnpconNoTblAutoRight
{
	int dummy;
	[NDORelation]
	CmpBin1OwnpconNoTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1OwnpconNoTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBin1OwnpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBin1OwnpconTblAutoRight), RelationInfo.Composite), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1OwnpconTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpBin1OwnpconTblAutoLeftDerived : CmpBin1OwnpconTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpBin1OwnpconTblAutoRight
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBin1OwnpconTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1OwnpconTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi1nOwnpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi1nOwnpconTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nOwnpconTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nOwnpconTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi1nOwnpconTblAutoLeftDerived : CmpBi1nOwnpconTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpBi1nOwnpconTblAutoRight
{
	int dummy;
	[NDORelation(typeof(CmpBi1nOwnpconTblAutoLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBinnOwnpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBinnOwnpconTblAutoRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBinnOwnpconTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpBinnOwnpconTblAutoLeftDerived : CmpBinnOwnpconTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpBinnOwnpconTblAutoRight
{
	int dummy;
	[NDORelation(typeof(CmpBinnOwnpconTblAutoLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrDir1OthpconNoTblAutoLeft
{
	int dummy;
	[NDORelation]
	AgrDir1OthpconNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OthpconNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OthpconNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrDir1OthpconNoTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrDir1OthpconNoTblAutoRightDerived : AgrDir1OthpconNoTblAutoRightBase
{
}


[NDOPersistent]
public class AgrDir1OthpconTblAutoLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrDir1OthpconTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OthpconTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OthpconTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrDir1OthpconTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrDir1OthpconTblAutoRightDerived : AgrDir1OthpconTblAutoRightBase
{
}


[NDOPersistent]
public class AgrBi11OthpconNoTblAutoLeft
{
	int dummy;
	[NDORelation]
	AgrBi11OthpconNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OthpconNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OthpconNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi11OthpconNoTblAutoRightBase
{
	int dummy;
	[NDORelation]
	AgrBi11OthpconNoTblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OthpconNoTblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi11OthpconNoTblAutoRightDerived : AgrBi11OthpconNoTblAutoRightBase
{
}


[NDOPersistent]
public class AgrBi11OthpconTblAutoLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OthpconTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OthpconTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OthpconTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi11OthpconTblAutoRightBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OthpconTblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OthpconTblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi11OthpconTblAutoRightDerived : AgrBi11OthpconTblAutoRightBase
{
}


[NDOPersistent]
public class AgrDirnOthpconTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(AgrDirnOthpconTblAutoRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnOthpconTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrDirnOthpconTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrDirnOthpconTblAutoRightDerived : AgrDirnOthpconTblAutoRightBase
{
}


[NDOPersistent]
public class AgrBin1OthpconTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(AgrBin1OthpconTblAutoRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1OthpconTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrBin1OthpconTblAutoRightBase
{
	int dummy;
	[NDORelation]
	AgrBin1OthpconTblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1OthpconTblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBin1OthpconTblAutoRightDerived : AgrBin1OthpconTblAutoRightBase
{
}


[NDOPersistent]
public class AgrBi1nOthpconNoTblAutoLeft
{
	int dummy;
	[NDORelation]
	AgrBi1nOthpconNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nOthpconNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nOthpconNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi1nOthpconNoTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBi1nOthpconNoTblAutoLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi1nOthpconNoTblAutoRightDerived : AgrBi1nOthpconNoTblAutoRightBase
{
}


[NDOPersistent]
public class AgrBi1nOthpconTblAutoLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi1nOthpconTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nOthpconTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nOthpconTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi1nOthpconTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBi1nOthpconTblAutoLeft)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi1nOthpconTblAutoRightDerived : AgrBi1nOthpconTblAutoRightBase
{
}


[NDOPersistent]
public class AgrBinnOthpconTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(AgrBinnOthpconTblAutoRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBinnOthpconTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrBinnOthpconTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBinnOthpconTblAutoLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBinnOthpconTblAutoRightDerived : AgrBinnOthpconTblAutoRightBase
{
}


[NDOPersistent]
public class CmpDir1OthpconNoTblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpDir1OthpconNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OthpconNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OthpconNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpDir1OthpconNoTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpDir1OthpconNoTblAutoRightDerived : CmpDir1OthpconNoTblAutoRightBase
{
}


[NDOPersistent]
public class CmpDir1OthpconTblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpDir1OthpconTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OthpconTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OthpconTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpDir1OthpconTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpDir1OthpconTblAutoRightDerived : CmpDir1OthpconTblAutoRightBase
{
}


[NDOPersistent]
public class CmpBi11OthpconNoTblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi11OthpconNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OthpconNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OthpconNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi11OthpconNoTblAutoRightBase
{
	int dummy;
	[NDORelation]
	CmpBi11OthpconNoTblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OthpconNoTblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi11OthpconNoTblAutoRightDerived : CmpBi11OthpconNoTblAutoRightBase
{
}


[NDOPersistent]
public class CmpBi11OthpconTblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi11OthpconTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OthpconTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OthpconTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi11OthpconTblAutoRightBase
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBi11OthpconTblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OthpconTblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi11OthpconTblAutoRightDerived : CmpBi11OthpconTblAutoRightBase
{
}


[NDOPersistent]
public class CmpDirnOthpconTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(CmpDirnOthpconTblAutoRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnOthpconTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpDirnOthpconTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpDirnOthpconTblAutoRightDerived : CmpDirnOthpconTblAutoRightBase
{
}


[NDOPersistent]
public class CmpBin1OthpconTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(CmpBin1OthpconTblAutoRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1OthpconTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpBin1OthpconTblAutoRightBase
{
	int dummy;
	[NDORelation]
	CmpBin1OthpconTblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1OthpconTblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBin1OthpconTblAutoRightDerived : CmpBin1OthpconTblAutoRightBase
{
}


[NDOPersistent]
public class CmpBi1nOthpconNoTblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi1nOthpconNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nOthpconNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nOthpconNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi1nOthpconNoTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBi1nOthpconNoTblAutoLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi1nOthpconNoTblAutoRightDerived : CmpBi1nOthpconNoTblAutoRightBase
{
}


[NDOPersistent]
public class CmpBi1nOthpconTblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi1nOthpconTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nOthpconTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nOthpconTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi1nOthpconTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBi1nOthpconTblAutoLeft)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi1nOthpconTblAutoRightDerived : CmpBi1nOthpconTblAutoRightBase
{
}


[NDOPersistent]
public class CmpBinnOthpconTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(CmpBinnOthpconTblAutoRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBinnOthpconTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpBinnOthpconTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBinnOthpconTblAutoLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBinnOthpconTblAutoRightDerived : CmpBinnOthpconTblAutoRightBase
{
}


[NDOPersistent]
public class AgrDir1OwnpconOthpconNoTblAutoLeftBase
{
	int dummy;
	[NDORelation]
	AgrDir1OwnpconOthpconNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OwnpconOthpconNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OwnpconOthpconNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrDir1OwnpconOthpconNoTblAutoLeftDerived : AgrDir1OwnpconOthpconNoTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrDir1OwnpconOthpconNoTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrDir1OwnpconOthpconNoTblAutoRightDerived : AgrDir1OwnpconOthpconNoTblAutoRightBase
{
}


[NDOPersistent]
public class AgrDir1OwnpconOthpconTblAutoLeftBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrDir1OwnpconOthpconTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OwnpconOthpconTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OwnpconOthpconTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrDir1OwnpconOthpconTblAutoLeftDerived : AgrDir1OwnpconOthpconTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrDir1OwnpconOthpconTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrDir1OwnpconOthpconTblAutoRightDerived : AgrDir1OwnpconOthpconTblAutoRightBase
{
}


[NDOPersistent]
public class AgrBi11OwnpconOthpconNoTblAutoLeftBase
{
	int dummy;
	[NDORelation]
	AgrBi11OwnpconOthpconNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpconOthpconNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OwnpconOthpconNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi11OwnpconOthpconNoTblAutoLeftDerived : AgrBi11OwnpconOthpconNoTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrBi11OwnpconOthpconNoTblAutoRightBase
{
	int dummy;
	[NDORelation]
	AgrBi11OwnpconOthpconNoTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpconOthpconNoTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi11OwnpconOthpconNoTblAutoRightDerived : AgrBi11OwnpconOthpconNoTblAutoRightBase
{
}


[NDOPersistent]
public class AgrBi11OwnpconOthpconTblAutoLeftBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OwnpconOthpconTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpconOthpconTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OwnpconOthpconTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi11OwnpconOthpconTblAutoLeftDerived : AgrBi11OwnpconOthpconTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrBi11OwnpconOthpconTblAutoRightBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OwnpconOthpconTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpconOthpconTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi11OwnpconOthpconTblAutoRightDerived : AgrBi11OwnpconOthpconTblAutoRightBase
{
}


[NDOPersistent]
public class AgrDirnOwnpconOthpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrDirnOwnpconOthpconTblAutoRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnOwnpconOthpconTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrDirnOwnpconOthpconTblAutoLeftDerived : AgrDirnOwnpconOthpconTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrDirnOwnpconOthpconTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrDirnOwnpconOthpconTblAutoRightDerived : AgrDirnOwnpconOthpconTblAutoRightBase
{
}


[NDOPersistent]
public class AgrBin1OwnpconOthpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBin1OwnpconOthpconTblAutoRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1OwnpconOthpconTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrBin1OwnpconOthpconTblAutoLeftDerived : AgrBin1OwnpconOthpconTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrBin1OwnpconOthpconTblAutoRightBase
{
	int dummy;
	[NDORelation]
	AgrBin1OwnpconOthpconTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1OwnpconOthpconTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBin1OwnpconOthpconTblAutoRightDerived : AgrBin1OwnpconOthpconTblAutoRightBase
{
}


[NDOPersistent]
public class AgrBi1nOwnpconOthpconTblAutoLeftBase
{
	int dummy;
	[NDORelation]
	AgrBi1nOwnpconOthpconTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nOwnpconOthpconTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nOwnpconOthpconTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi1nOwnpconOthpconTblAutoLeftDerived : AgrBi1nOwnpconOthpconTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrBi1nOwnpconOthpconTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBi1nOwnpconOthpconTblAutoLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi1nOwnpconOthpconTblAutoRightDerived : AgrBi1nOwnpconOthpconTblAutoRightBase
{
}


[NDOPersistent]
public class AgrBinnOwnpconOthpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBinnOwnpconOthpconTblAutoRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBinnOwnpconOthpconTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrBinnOwnpconOthpconTblAutoLeftDerived : AgrBinnOwnpconOthpconTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrBinnOwnpconOthpconTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBinnOwnpconOthpconTblAutoLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBinnOwnpconOthpconTblAutoRightDerived : AgrBinnOwnpconOthpconTblAutoRightBase
{
}


[NDOPersistent]
public class CmpDir1OwnpconOthpconNoTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpDir1OwnpconOthpconNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OwnpconOthpconNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OwnpconOthpconNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpDir1OwnpconOthpconNoTblAutoLeftDerived : CmpDir1OwnpconOthpconNoTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpDir1OwnpconOthpconNoTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpDir1OwnpconOthpconNoTblAutoRightDerived : CmpDir1OwnpconOthpconNoTblAutoRightBase
{
}


[NDOPersistent]
public class CmpDir1OwnpconOthpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpDir1OwnpconOthpconTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OwnpconOthpconTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OwnpconOthpconTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpDir1OwnpconOthpconTblAutoLeftDerived : CmpDir1OwnpconOthpconTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpDir1OwnpconOthpconTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpDir1OwnpconOthpconTblAutoRightDerived : CmpDir1OwnpconOthpconTblAutoRightBase
{
}


[NDOPersistent]
public class CmpBi11OwnpconOthpconNoTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi11OwnpconOthpconNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpconOthpconNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OwnpconOthpconNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi11OwnpconOthpconNoTblAutoLeftDerived : CmpBi11OwnpconOthpconNoTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpBi11OwnpconOthpconNoTblAutoRightBase
{
	int dummy;
	[NDORelation]
	CmpBi11OwnpconOthpconNoTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpconOthpconNoTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi11OwnpconOthpconNoTblAutoRightDerived : CmpBi11OwnpconOthpconNoTblAutoRightBase
{
}


[NDOPersistent]
public class CmpBi11OwnpconOthpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi11OwnpconOthpconTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpconOthpconTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OwnpconOthpconTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi11OwnpconOthpconTblAutoLeftDerived : CmpBi11OwnpconOthpconTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpBi11OwnpconOthpconTblAutoRightBase
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBi11OwnpconOthpconTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpconOthpconTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi11OwnpconOthpconTblAutoRightDerived : CmpBi11OwnpconOthpconTblAutoRightBase
{
}


[NDOPersistent]
public class CmpDirnOwnpconOthpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpDirnOwnpconOthpconTblAutoRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnOwnpconOthpconTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpDirnOwnpconOthpconTblAutoLeftDerived : CmpDirnOwnpconOthpconTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpDirnOwnpconOthpconTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpDirnOwnpconOthpconTblAutoRightDerived : CmpDirnOwnpconOthpconTblAutoRightBase
{
}


[NDOPersistent]
public class CmpBin1OwnpconOthpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBin1OwnpconOthpconTblAutoRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1OwnpconOthpconTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpBin1OwnpconOthpconTblAutoLeftDerived : CmpBin1OwnpconOthpconTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpBin1OwnpconOthpconTblAutoRightBase
{
	int dummy;
	[NDORelation]
	CmpBin1OwnpconOthpconTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1OwnpconOthpconTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBin1OwnpconOthpconTblAutoRightDerived : CmpBin1OwnpconOthpconTblAutoRightBase
{
}


[NDOPersistent]
public class CmpBi1nOwnpconOthpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi1nOwnpconOthpconTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nOwnpconOthpconTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nOwnpconOthpconTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi1nOwnpconOthpconTblAutoLeftDerived : CmpBi1nOwnpconOthpconTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpBi1nOwnpconOthpconTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBi1nOwnpconOthpconTblAutoLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi1nOwnpconOthpconTblAutoRightDerived : CmpBi1nOwnpconOthpconTblAutoRightBase
{
}


[NDOPersistent]
public class CmpBinnOwnpconOthpconTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBinnOwnpconOthpconTblAutoRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBinnOwnpconOthpconTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpBinnOwnpconOthpconTblAutoLeftDerived : CmpBinnOwnpconOthpconTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpBinnOwnpconOthpconTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBinnOwnpconOthpconTblAutoLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBinnOwnpconOthpconTblAutoRightDerived : CmpBinnOwnpconOthpconTblAutoRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1NoTblGuidLeft
{
	int dummy;
	[NDORelation]
	AgrDir1NoTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1NoTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1NoTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1NoTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1TblGuidLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrDir1TblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1TblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1TblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1TblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11NoTblGuidLeft
{
	int dummy;
	[NDORelation]
	AgrBi11NoTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11NoTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11NoTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11NoTblGuidRight
{
	int dummy;
	[NDORelation]
	AgrBi11NoTblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11NoTblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11TblGuidLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11TblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11TblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11TblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11TblGuidRight
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11TblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11TblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnNoTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(AgrDirnNoTblGuidRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnNoTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnNoTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(AgrDirnTblGuidRight)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1NoTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(AgrBin1NoTblGuidRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1NoTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1NoTblGuidRight
{
	int dummy;
	[NDORelation]
	AgrBin1NoTblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1NoTblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1TblGuidLeft
{
	int dummy;
	[NDORelation(typeof(AgrBin1TblGuidRight)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1TblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1TblGuidRight
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBin1TblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1TblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nNoTblGuidLeft
{
	int dummy;
	[NDORelation]
	AgrBi1nNoTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nNoTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nNoTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nNoTblGuidRight
{
	int dummy;
	[NDORelation(typeof(AgrBi1nNoTblGuidLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nTblGuidLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi1nTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nTblGuidRight
{
	int dummy;
	[NDORelation(typeof(AgrBi1nTblGuidLeft)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(AgrBinnTblGuidRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBinnTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnTblGuidRight
{
	int dummy;
	[NDORelation(typeof(AgrBinnTblGuidLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1NoTblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpDir1NoTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1NoTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1NoTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1NoTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1TblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpDir1TblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1TblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1TblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1TblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11NoTblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi11NoTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11NoTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11NoTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11NoTblGuidRight
{
	int dummy;
	[NDORelation]
	CmpBi11NoTblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11NoTblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11TblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi11TblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11TblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11TblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11TblGuidRight
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBi11TblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11TblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnNoTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(CmpDirnNoTblGuidRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnNoTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnNoTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(CmpDirnTblGuidRight), RelationInfo.Composite), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1NoTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(CmpBin1NoTblGuidRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1NoTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1NoTblGuidRight
{
	int dummy;
	[NDORelation]
	CmpBin1NoTblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1NoTblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1TblGuidLeft
{
	int dummy;
	[NDORelation(typeof(CmpBin1TblGuidRight), RelationInfo.Composite), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1TblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1TblGuidRight
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBin1TblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1TblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nNoTblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi1nNoTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nNoTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nNoTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nNoTblGuidRight
{
	int dummy;
	[NDORelation(typeof(CmpBi1nNoTblGuidLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nTblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi1nTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nTblGuidRight
{
	int dummy;
	[NDORelation(typeof(CmpBi1nTblGuidLeft)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(CmpBinnTblGuidRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBinnTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnTblGuidRight
{
	int dummy;
	[NDORelation(typeof(CmpBinnTblGuidLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpconNoTblGuidLeftBase
{
	int dummy;
	[NDORelation]
	AgrDir1OwnpconNoTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OwnpconNoTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OwnpconNoTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpconNoTblGuidLeftDerived : AgrDir1OwnpconNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpconNoTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpconTblGuidLeftBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrDir1OwnpconTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OwnpconTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OwnpconTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpconTblGuidLeftDerived : AgrDir1OwnpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpconTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpconNoTblGuidLeftBase
{
	int dummy;
	[NDORelation]
	AgrBi11OwnpconNoTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpconNoTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OwnpconNoTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpconNoTblGuidLeftDerived : AgrBi11OwnpconNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpconNoTblGuidRight
{
	int dummy;
	[NDORelation]
	AgrBi11OwnpconNoTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpconNoTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpconTblGuidLeftBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OwnpconTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpconTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OwnpconTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpconTblGuidLeftDerived : AgrBi11OwnpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpconTblGuidRight
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OwnpconTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpconTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOwnpconNoTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrDirnOwnpconNoTblGuidRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnOwnpconNoTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOwnpconNoTblGuidLeftDerived : AgrDirnOwnpconNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOwnpconNoTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOwnpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrDirnOwnpconTblGuidRight)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnOwnpconTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOwnpconTblGuidLeftDerived : AgrDirnOwnpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOwnpconTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OwnpconNoTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBin1OwnpconNoTblGuidRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1OwnpconNoTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OwnpconNoTblGuidLeftDerived : AgrBin1OwnpconNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OwnpconNoTblGuidRight
{
	int dummy;
	[NDORelation]
	AgrBin1OwnpconNoTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1OwnpconNoTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OwnpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBin1OwnpconTblGuidRight)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1OwnpconTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OwnpconTblGuidLeftDerived : AgrBin1OwnpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OwnpconTblGuidRight
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBin1OwnpconTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1OwnpconTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOwnpconTblGuidLeftBase
{
	int dummy;
	[NDORelation]
	AgrBi1nOwnpconTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nOwnpconTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nOwnpconTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOwnpconTblGuidLeftDerived : AgrBi1nOwnpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOwnpconTblGuidRight
{
	int dummy;
	[NDORelation(typeof(AgrBi1nOwnpconTblGuidLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnOwnpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBinnOwnpconTblGuidRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBinnOwnpconTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnOwnpconTblGuidLeftDerived : AgrBinnOwnpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnOwnpconTblGuidRight
{
	int dummy;
	[NDORelation(typeof(AgrBinnOwnpconTblGuidLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpconNoTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpDir1OwnpconNoTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OwnpconNoTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OwnpconNoTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpconNoTblGuidLeftDerived : CmpDir1OwnpconNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpconNoTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpDir1OwnpconTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OwnpconTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OwnpconTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpconTblGuidLeftDerived : CmpDir1OwnpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpconTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpconNoTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi11OwnpconNoTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpconNoTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OwnpconNoTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpconNoTblGuidLeftDerived : CmpBi11OwnpconNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpconNoTblGuidRight
{
	int dummy;
	[NDORelation]
	CmpBi11OwnpconNoTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpconNoTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi11OwnpconTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpconTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OwnpconTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpconTblGuidLeftDerived : CmpBi11OwnpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpconTblGuidRight
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBi11OwnpconTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpconTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOwnpconNoTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpDirnOwnpconNoTblGuidRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnOwnpconNoTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOwnpconNoTblGuidLeftDerived : CmpDirnOwnpconNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOwnpconNoTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOwnpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpDirnOwnpconTblGuidRight), RelationInfo.Composite), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnOwnpconTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOwnpconTblGuidLeftDerived : CmpDirnOwnpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOwnpconTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OwnpconNoTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBin1OwnpconNoTblGuidRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1OwnpconNoTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OwnpconNoTblGuidLeftDerived : CmpBin1OwnpconNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OwnpconNoTblGuidRight
{
	int dummy;
	[NDORelation]
	CmpBin1OwnpconNoTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1OwnpconNoTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OwnpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBin1OwnpconTblGuidRight), RelationInfo.Composite), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1OwnpconTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OwnpconTblGuidLeftDerived : CmpBin1OwnpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OwnpconTblGuidRight
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBin1OwnpconTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1OwnpconTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOwnpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi1nOwnpconTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nOwnpconTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nOwnpconTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOwnpconTblGuidLeftDerived : CmpBi1nOwnpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOwnpconTblGuidRight
{
	int dummy;
	[NDORelation(typeof(CmpBi1nOwnpconTblGuidLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnOwnpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBinnOwnpconTblGuidRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBinnOwnpconTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnOwnpconTblGuidLeftDerived : CmpBinnOwnpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnOwnpconTblGuidRight
{
	int dummy;
	[NDORelation(typeof(CmpBinnOwnpconTblGuidLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OthpconNoTblGuidLeft
{
	int dummy;
	[NDORelation]
	AgrDir1OthpconNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OthpconNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OthpconNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OthpconNoTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OthpconNoTblGuidRightDerived : AgrDir1OthpconNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OthpconTblGuidLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrDir1OthpconTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OthpconTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OthpconTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OthpconTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OthpconTblGuidRightDerived : AgrDir1OthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OthpconNoTblGuidLeft
{
	int dummy;
	[NDORelation]
	AgrBi11OthpconNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OthpconNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OthpconNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OthpconNoTblGuidRightBase
{
	int dummy;
	[NDORelation]
	AgrBi11OthpconNoTblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OthpconNoTblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OthpconNoTblGuidRightDerived : AgrBi11OthpconNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OthpconTblGuidLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OthpconTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OthpconTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OthpconTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OthpconTblGuidRightBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OthpconTblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OthpconTblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OthpconTblGuidRightDerived : AgrBi11OthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOthpconTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(AgrDirnOthpconTblGuidRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnOthpconTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOthpconTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOthpconTblGuidRightDerived : AgrDirnOthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OthpconTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(AgrBin1OthpconTblGuidRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1OthpconTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OthpconTblGuidRightBase
{
	int dummy;
	[NDORelation]
	AgrBin1OthpconTblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1OthpconTblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OthpconTblGuidRightDerived : AgrBin1OthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOthpconNoTblGuidLeft
{
	int dummy;
	[NDORelation]
	AgrBi1nOthpconNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nOthpconNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nOthpconNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOthpconNoTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBi1nOthpconNoTblGuidLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOthpconNoTblGuidRightDerived : AgrBi1nOthpconNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOthpconTblGuidLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi1nOthpconTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nOthpconTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nOthpconTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOthpconTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBi1nOthpconTblGuidLeft)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOthpconTblGuidRightDerived : AgrBi1nOthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnOthpconTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(AgrBinnOthpconTblGuidRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBinnOthpconTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnOthpconTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBinnOthpconTblGuidLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnOthpconTblGuidRightDerived : AgrBinnOthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OthpconNoTblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpDir1OthpconNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OthpconNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OthpconNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OthpconNoTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OthpconNoTblGuidRightDerived : CmpDir1OthpconNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OthpconTblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpDir1OthpconTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OthpconTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OthpconTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OthpconTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OthpconTblGuidRightDerived : CmpDir1OthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OthpconNoTblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi11OthpconNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OthpconNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OthpconNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OthpconNoTblGuidRightBase
{
	int dummy;
	[NDORelation]
	CmpBi11OthpconNoTblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OthpconNoTblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OthpconNoTblGuidRightDerived : CmpBi11OthpconNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OthpconTblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi11OthpconTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OthpconTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OthpconTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OthpconTblGuidRightBase
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBi11OthpconTblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OthpconTblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OthpconTblGuidRightDerived : CmpBi11OthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOthpconTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(CmpDirnOthpconTblGuidRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnOthpconTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOthpconTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOthpconTblGuidRightDerived : CmpDirnOthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OthpconTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(CmpBin1OthpconTblGuidRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1OthpconTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OthpconTblGuidRightBase
{
	int dummy;
	[NDORelation]
	CmpBin1OthpconTblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1OthpconTblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OthpconTblGuidRightDerived : CmpBin1OthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOthpconNoTblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi1nOthpconNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nOthpconNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nOthpconNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOthpconNoTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBi1nOthpconNoTblGuidLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOthpconNoTblGuidRightDerived : CmpBi1nOthpconNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOthpconTblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi1nOthpconTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nOthpconTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nOthpconTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOthpconTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBi1nOthpconTblGuidLeft)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOthpconTblGuidRightDerived : CmpBi1nOthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnOthpconTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(CmpBinnOthpconTblGuidRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBinnOthpconTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnOthpconTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBinnOthpconTblGuidLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnOthpconTblGuidRightDerived : CmpBinnOthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpconOthpconNoTblGuidLeftBase
{
	int dummy;
	[NDORelation]
	AgrDir1OwnpconOthpconNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OwnpconOthpconNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OwnpconOthpconNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpconOthpconNoTblGuidLeftDerived : AgrDir1OwnpconOthpconNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpconOthpconNoTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpconOthpconNoTblGuidRightDerived : AgrDir1OwnpconOthpconNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpconOthpconTblGuidLeftBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrDir1OwnpconOthpconTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OwnpconOthpconTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OwnpconOthpconTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpconOthpconTblGuidLeftDerived : AgrDir1OwnpconOthpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpconOthpconTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpconOthpconTblGuidRightDerived : AgrDir1OwnpconOthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpconOthpconNoTblGuidLeftBase
{
	int dummy;
	[NDORelation]
	AgrBi11OwnpconOthpconNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpconOthpconNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OwnpconOthpconNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpconOthpconNoTblGuidLeftDerived : AgrBi11OwnpconOthpconNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpconOthpconNoTblGuidRightBase
{
	int dummy;
	[NDORelation]
	AgrBi11OwnpconOthpconNoTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpconOthpconNoTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpconOthpconNoTblGuidRightDerived : AgrBi11OwnpconOthpconNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpconOthpconTblGuidLeftBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OwnpconOthpconTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpconOthpconTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OwnpconOthpconTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpconOthpconTblGuidLeftDerived : AgrBi11OwnpconOthpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpconOthpconTblGuidRightBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OwnpconOthpconTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpconOthpconTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpconOthpconTblGuidRightDerived : AgrBi11OwnpconOthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOwnpconOthpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrDirnOwnpconOthpconTblGuidRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnOwnpconOthpconTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOwnpconOthpconTblGuidLeftDerived : AgrDirnOwnpconOthpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOwnpconOthpconTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOwnpconOthpconTblGuidRightDerived : AgrDirnOwnpconOthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OwnpconOthpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBin1OwnpconOthpconTblGuidRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1OwnpconOthpconTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OwnpconOthpconTblGuidLeftDerived : AgrBin1OwnpconOthpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OwnpconOthpconTblGuidRightBase
{
	int dummy;
	[NDORelation]
	AgrBin1OwnpconOthpconTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1OwnpconOthpconTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OwnpconOthpconTblGuidRightDerived : AgrBin1OwnpconOthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOwnpconOthpconTblGuidLeftBase
{
	int dummy;
	[NDORelation]
	AgrBi1nOwnpconOthpconTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nOwnpconOthpconTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nOwnpconOthpconTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOwnpconOthpconTblGuidLeftDerived : AgrBi1nOwnpconOthpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOwnpconOthpconTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBi1nOwnpconOthpconTblGuidLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOwnpconOthpconTblGuidRightDerived : AgrBi1nOwnpconOthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnOwnpconOthpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBinnOwnpconOthpconTblGuidRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBinnOwnpconOthpconTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnOwnpconOthpconTblGuidLeftDerived : AgrBinnOwnpconOthpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnOwnpconOthpconTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBinnOwnpconOthpconTblGuidLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnOwnpconOthpconTblGuidRightDerived : AgrBinnOwnpconOthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpconOthpconNoTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpDir1OwnpconOthpconNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OwnpconOthpconNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OwnpconOthpconNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpconOthpconNoTblGuidLeftDerived : CmpDir1OwnpconOthpconNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpconOthpconNoTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpconOthpconNoTblGuidRightDerived : CmpDir1OwnpconOthpconNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpconOthpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpDir1OwnpconOthpconTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OwnpconOthpconTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OwnpconOthpconTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpconOthpconTblGuidLeftDerived : CmpDir1OwnpconOthpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpconOthpconTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpconOthpconTblGuidRightDerived : CmpDir1OwnpconOthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpconOthpconNoTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi11OwnpconOthpconNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpconOthpconNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OwnpconOthpconNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpconOthpconNoTblGuidLeftDerived : CmpBi11OwnpconOthpconNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpconOthpconNoTblGuidRightBase
{
	int dummy;
	[NDORelation]
	CmpBi11OwnpconOthpconNoTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpconOthpconNoTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpconOthpconNoTblGuidRightDerived : CmpBi11OwnpconOthpconNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpconOthpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi11OwnpconOthpconTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpconOthpconTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OwnpconOthpconTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpconOthpconTblGuidLeftDerived : CmpBi11OwnpconOthpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpconOthpconTblGuidRightBase
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBi11OwnpconOthpconTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpconOthpconTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpconOthpconTblGuidRightDerived : CmpBi11OwnpconOthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOwnpconOthpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpDirnOwnpconOthpconTblGuidRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnOwnpconOthpconTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOwnpconOthpconTblGuidLeftDerived : CmpDirnOwnpconOthpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOwnpconOthpconTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOwnpconOthpconTblGuidRightDerived : CmpDirnOwnpconOthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OwnpconOthpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBin1OwnpconOthpconTblGuidRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1OwnpconOthpconTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OwnpconOthpconTblGuidLeftDerived : CmpBin1OwnpconOthpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OwnpconOthpconTblGuidRightBase
{
	int dummy;
	[NDORelation]
	CmpBin1OwnpconOthpconTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1OwnpconOthpconTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OwnpconOthpconTblGuidRightDerived : CmpBin1OwnpconOthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOwnpconOthpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi1nOwnpconOthpconTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nOwnpconOthpconTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nOwnpconOthpconTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOwnpconOthpconTblGuidLeftDerived : CmpBi1nOwnpconOthpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOwnpconOthpconTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBi1nOwnpconOthpconTblGuidLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOwnpconOthpconTblGuidRightDerived : CmpBi1nOwnpconOthpconTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnOwnpconOthpconTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBinnOwnpconOthpconTblGuidRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBinnOwnpconOthpconTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnOwnpconOthpconTblGuidLeftDerived : CmpBinnOwnpconOthpconTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnOwnpconOthpconTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBinnOwnpconOthpconTblGuidLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnOwnpconOthpconTblGuidRightDerived : CmpBinnOwnpconOthpconTblGuidRightBase
{
}


[NDOPersistent]
public abstract class AgrDir1OwnpabsNoTblAutoLeftBase
{
	int dummy;
	[NDORelation]
	AgrDir1OwnpabsNoTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OwnpabsNoTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OwnpabsNoTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrDir1OwnpabsNoTblAutoLeftDerived : AgrDir1OwnpabsNoTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrDir1OwnpabsNoTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public abstract class AgrDir1OwnpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrDir1OwnpabsTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OwnpabsTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OwnpabsTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrDir1OwnpabsTblAutoLeftDerived : AgrDir1OwnpabsTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrDir1OwnpabsTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public abstract class AgrBi11OwnpabsNoTblAutoLeftBase
{
	int dummy;
	[NDORelation]
	AgrBi11OwnpabsNoTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpabsNoTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OwnpabsNoTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi11OwnpabsNoTblAutoLeftDerived : AgrBi11OwnpabsNoTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrBi11OwnpabsNoTblAutoRight
{
	int dummy;
	[NDORelation]
	AgrBi11OwnpabsNoTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpabsNoTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public abstract class AgrBi11OwnpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OwnpabsTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpabsTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OwnpabsTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi11OwnpabsTblAutoLeftDerived : AgrBi11OwnpabsTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrBi11OwnpabsTblAutoRight
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OwnpabsTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpabsTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public abstract class AgrDirnOwnpabsNoTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrDirnOwnpabsNoTblAutoRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnOwnpabsNoTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrDirnOwnpabsNoTblAutoLeftDerived : AgrDirnOwnpabsNoTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrDirnOwnpabsNoTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public abstract class AgrDirnOwnpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrDirnOwnpabsTblAutoRight)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnOwnpabsTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrDirnOwnpabsTblAutoLeftDerived : AgrDirnOwnpabsTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrDirnOwnpabsTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public abstract class AgrBin1OwnpabsNoTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBin1OwnpabsNoTblAutoRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1OwnpabsNoTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrBin1OwnpabsNoTblAutoLeftDerived : AgrBin1OwnpabsNoTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrBin1OwnpabsNoTblAutoRight
{
	int dummy;
	[NDORelation]
	AgrBin1OwnpabsNoTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1OwnpabsNoTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public abstract class AgrBin1OwnpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBin1OwnpabsTblAutoRight)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1OwnpabsTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrBin1OwnpabsTblAutoLeftDerived : AgrBin1OwnpabsTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrBin1OwnpabsTblAutoRight
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBin1OwnpabsTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1OwnpabsTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public abstract class AgrBi1nOwnpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation]
	AgrBi1nOwnpabsTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nOwnpabsTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nOwnpabsTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi1nOwnpabsTblAutoLeftDerived : AgrBi1nOwnpabsTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrBi1nOwnpabsTblAutoRight
{
	int dummy;
	[NDORelation(typeof(AgrBi1nOwnpabsTblAutoLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public abstract class AgrBinnOwnpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBinnOwnpabsTblAutoRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBinnOwnpabsTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrBinnOwnpabsTblAutoLeftDerived : AgrBinnOwnpabsTblAutoLeftBase
{
}


[NDOPersistent]
public class AgrBinnOwnpabsTblAutoRight
{
	int dummy;
	[NDORelation(typeof(AgrBinnOwnpabsTblAutoLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public abstract class CmpDir1OwnpabsNoTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpDir1OwnpabsNoTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OwnpabsNoTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OwnpabsNoTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpDir1OwnpabsNoTblAutoLeftDerived : CmpDir1OwnpabsNoTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpDir1OwnpabsNoTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public abstract class CmpDir1OwnpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpDir1OwnpabsTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OwnpabsTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OwnpabsTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpDir1OwnpabsTblAutoLeftDerived : CmpDir1OwnpabsTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpDir1OwnpabsTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public abstract class CmpBi11OwnpabsNoTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi11OwnpabsNoTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpabsNoTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OwnpabsNoTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi11OwnpabsNoTblAutoLeftDerived : CmpBi11OwnpabsNoTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpBi11OwnpabsNoTblAutoRight
{
	int dummy;
	[NDORelation]
	CmpBi11OwnpabsNoTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpabsNoTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public abstract class CmpBi11OwnpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi11OwnpabsTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpabsTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OwnpabsTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi11OwnpabsTblAutoLeftDerived : CmpBi11OwnpabsTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpBi11OwnpabsTblAutoRight
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBi11OwnpabsTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpabsTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public abstract class CmpDirnOwnpabsNoTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpDirnOwnpabsNoTblAutoRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnOwnpabsNoTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpDirnOwnpabsNoTblAutoLeftDerived : CmpDirnOwnpabsNoTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpDirnOwnpabsNoTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public abstract class CmpDirnOwnpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpDirnOwnpabsTblAutoRight), RelationInfo.Composite), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnOwnpabsTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpDirnOwnpabsTblAutoLeftDerived : CmpDirnOwnpabsTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpDirnOwnpabsTblAutoRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public abstract class CmpBin1OwnpabsNoTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBin1OwnpabsNoTblAutoRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1OwnpabsNoTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpBin1OwnpabsNoTblAutoLeftDerived : CmpBin1OwnpabsNoTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpBin1OwnpabsNoTblAutoRight
{
	int dummy;
	[NDORelation]
	CmpBin1OwnpabsNoTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1OwnpabsNoTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public abstract class CmpBin1OwnpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBin1OwnpabsTblAutoRight), RelationInfo.Composite), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1OwnpabsTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpBin1OwnpabsTblAutoLeftDerived : CmpBin1OwnpabsTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpBin1OwnpabsTblAutoRight
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBin1OwnpabsTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1OwnpabsTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public abstract class CmpBi1nOwnpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi1nOwnpabsTblAutoRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nOwnpabsTblAutoRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nOwnpabsTblAutoRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi1nOwnpabsTblAutoLeftDerived : CmpBi1nOwnpabsTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpBi1nOwnpabsTblAutoRight
{
	int dummy;
	[NDORelation(typeof(CmpBi1nOwnpabsTblAutoLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public abstract class CmpBinnOwnpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBinnOwnpabsTblAutoRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBinnOwnpabsTblAutoRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpBinnOwnpabsTblAutoLeftDerived : CmpBinnOwnpabsTblAutoLeftBase
{
}


[NDOPersistent]
public class CmpBinnOwnpabsTblAutoRight
{
	int dummy;
	[NDORelation(typeof(CmpBinnOwnpabsTblAutoLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrDir1OthpabsNoTblAutoLeft
{
	int dummy;
	[NDORelation]
	AgrDir1OthpabsNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OthpabsNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OthpabsNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public abstract class AgrDir1OthpabsNoTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrDir1OthpabsNoTblAutoRightDerived : AgrDir1OthpabsNoTblAutoRightBase
{
}


[NDOPersistent]
public class AgrDir1OthpabsTblAutoLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrDir1OthpabsTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OthpabsTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OthpabsTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public abstract class AgrDir1OthpabsTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrDir1OthpabsTblAutoRightDerived : AgrDir1OthpabsTblAutoRightBase
{
}


[NDOPersistent]
public class AgrBi11OthpabsNoTblAutoLeft
{
	int dummy;
	[NDORelation]
	AgrBi11OthpabsNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OthpabsNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OthpabsNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public abstract class AgrBi11OthpabsNoTblAutoRightBase
{
	int dummy;
	[NDORelation]
	AgrBi11OthpabsNoTblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OthpabsNoTblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi11OthpabsNoTblAutoRightDerived : AgrBi11OthpabsNoTblAutoRightBase
{
}


[NDOPersistent]
public class AgrBi11OthpabsTblAutoLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OthpabsTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OthpabsTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OthpabsTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public abstract class AgrBi11OthpabsTblAutoRightBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OthpabsTblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OthpabsTblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi11OthpabsTblAutoRightDerived : AgrBi11OthpabsTblAutoRightBase
{
}


[NDOPersistent]
public class AgrDirnOthpabsTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(AgrDirnOthpabsTblAutoRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnOthpabsTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public abstract class AgrDirnOthpabsTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrDirnOthpabsTblAutoRightDerived : AgrDirnOthpabsTblAutoRightBase
{
}


[NDOPersistent]
public class AgrBin1OthpabsTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(AgrBin1OthpabsTblAutoRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1OthpabsTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public abstract class AgrBin1OthpabsTblAutoRightBase
{
	int dummy;
	[NDORelation]
	AgrBin1OthpabsTblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1OthpabsTblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBin1OthpabsTblAutoRightDerived : AgrBin1OthpabsTblAutoRightBase
{
}


[NDOPersistent]
public class AgrBi1nOthpabsNoTblAutoLeft
{
	int dummy;
	[NDORelation]
	AgrBi1nOthpabsNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nOthpabsNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nOthpabsNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public abstract class AgrBi1nOthpabsNoTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBi1nOthpabsNoTblAutoLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi1nOthpabsNoTblAutoRightDerived : AgrBi1nOthpabsNoTblAutoRightBase
{
}


[NDOPersistent]
public class AgrBi1nOthpabsTblAutoLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi1nOthpabsTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nOthpabsTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nOthpabsTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public abstract class AgrBi1nOthpabsTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBi1nOthpabsTblAutoLeft)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi1nOthpabsTblAutoRightDerived : AgrBi1nOthpabsTblAutoRightBase
{
}


[NDOPersistent]
public class AgrBinnOthpabsTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(AgrBinnOthpabsTblAutoRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBinnOthpabsTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public abstract class AgrBinnOthpabsTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBinnOthpabsTblAutoLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBinnOthpabsTblAutoRightDerived : AgrBinnOthpabsTblAutoRightBase
{
}


[NDOPersistent]
public class CmpDir1OthpabsNoTblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpDir1OthpabsNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OthpabsNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OthpabsNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public abstract class CmpDir1OthpabsNoTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpDir1OthpabsNoTblAutoRightDerived : CmpDir1OthpabsNoTblAutoRightBase
{
}


[NDOPersistent]
public class CmpDir1OthpabsTblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpDir1OthpabsTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OthpabsTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OthpabsTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public abstract class CmpDir1OthpabsTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpDir1OthpabsTblAutoRightDerived : CmpDir1OthpabsTblAutoRightBase
{
}


[NDOPersistent]
public class CmpBi11OthpabsNoTblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi11OthpabsNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OthpabsNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OthpabsNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public abstract class CmpBi11OthpabsNoTblAutoRightBase
{
	int dummy;
	[NDORelation]
	CmpBi11OthpabsNoTblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OthpabsNoTblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi11OthpabsNoTblAutoRightDerived : CmpBi11OthpabsNoTblAutoRightBase
{
}


[NDOPersistent]
public class CmpBi11OthpabsTblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi11OthpabsTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OthpabsTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OthpabsTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public abstract class CmpBi11OthpabsTblAutoRightBase
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBi11OthpabsTblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OthpabsTblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi11OthpabsTblAutoRightDerived : CmpBi11OthpabsTblAutoRightBase
{
}


[NDOPersistent]
public class CmpDirnOthpabsTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(CmpDirnOthpabsTblAutoRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnOthpabsTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public abstract class CmpDirnOthpabsTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpDirnOthpabsTblAutoRightDerived : CmpDirnOthpabsTblAutoRightBase
{
}


[NDOPersistent]
public class CmpBin1OthpabsTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(CmpBin1OthpabsTblAutoRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1OthpabsTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public abstract class CmpBin1OthpabsTblAutoRightBase
{
	int dummy;
	[NDORelation]
	CmpBin1OthpabsTblAutoLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1OthpabsTblAutoLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBin1OthpabsTblAutoRightDerived : CmpBin1OthpabsTblAutoRightBase
{
}


[NDOPersistent]
public class CmpBi1nOthpabsNoTblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi1nOthpabsNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nOthpabsNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nOthpabsNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public abstract class CmpBi1nOthpabsNoTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBi1nOthpabsNoTblAutoLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi1nOthpabsNoTblAutoRightDerived : CmpBi1nOthpabsNoTblAutoRightBase
{
}


[NDOPersistent]
public class CmpBi1nOthpabsTblAutoLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi1nOthpabsTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nOthpabsTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nOthpabsTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public abstract class CmpBi1nOthpabsTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBi1nOthpabsTblAutoLeft)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi1nOthpabsTblAutoRightDerived : CmpBi1nOthpabsTblAutoRightBase
{
}


[NDOPersistent]
public class CmpBinnOthpabsTblAutoLeft
{
	int dummy;
	[NDORelation(typeof(CmpBinnOthpabsTblAutoRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBinnOthpabsTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public abstract class CmpBinnOthpabsTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBinnOthpabsTblAutoLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBinnOthpabsTblAutoRightDerived : CmpBinnOthpabsTblAutoRightBase
{
}


[NDOPersistent]
public abstract class AgrDir1OwnpabsOthpabsNoTblAutoLeftBase
{
	int dummy;
	[NDORelation]
	AgrDir1OwnpabsOthpabsNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OwnpabsOthpabsNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OwnpabsOthpabsNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrDir1OwnpabsOthpabsNoTblAutoLeftDerived : AgrDir1OwnpabsOthpabsNoTblAutoLeftBase
{
}


[NDOPersistent]
public abstract class AgrDir1OwnpabsOthpabsNoTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrDir1OwnpabsOthpabsNoTblAutoRightDerived : AgrDir1OwnpabsOthpabsNoTblAutoRightBase
{
}


[NDOPersistent]
public abstract class AgrDir1OwnpabsOthpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrDir1OwnpabsOthpabsTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OwnpabsOthpabsTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OwnpabsOthpabsTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrDir1OwnpabsOthpabsTblAutoLeftDerived : AgrDir1OwnpabsOthpabsTblAutoLeftBase
{
}


[NDOPersistent]
public abstract class AgrDir1OwnpabsOthpabsTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrDir1OwnpabsOthpabsTblAutoRightDerived : AgrDir1OwnpabsOthpabsTblAutoRightBase
{
}


[NDOPersistent]
public abstract class AgrBi11OwnpabsOthpabsNoTblAutoLeftBase
{
	int dummy;
	[NDORelation]
	AgrBi11OwnpabsOthpabsNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpabsOthpabsNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OwnpabsOthpabsNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi11OwnpabsOthpabsNoTblAutoLeftDerived : AgrBi11OwnpabsOthpabsNoTblAutoLeftBase
{
}


[NDOPersistent]
public abstract class AgrBi11OwnpabsOthpabsNoTblAutoRightBase
{
	int dummy;
	[NDORelation]
	AgrBi11OwnpabsOthpabsNoTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpabsOthpabsNoTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi11OwnpabsOthpabsNoTblAutoRightDerived : AgrBi11OwnpabsOthpabsNoTblAutoRightBase
{
}


[NDOPersistent]
public abstract class AgrBi11OwnpabsOthpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OwnpabsOthpabsTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpabsOthpabsTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OwnpabsOthpabsTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi11OwnpabsOthpabsTblAutoLeftDerived : AgrBi11OwnpabsOthpabsTblAutoLeftBase
{
}


[NDOPersistent]
public abstract class AgrBi11OwnpabsOthpabsTblAutoRightBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OwnpabsOthpabsTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpabsOthpabsTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi11OwnpabsOthpabsTblAutoRightDerived : AgrBi11OwnpabsOthpabsTblAutoRightBase
{
}


[NDOPersistent]
public abstract class AgrDirnOwnpabsOthpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrDirnOwnpabsOthpabsTblAutoRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnOwnpabsOthpabsTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrDirnOwnpabsOthpabsTblAutoLeftDerived : AgrDirnOwnpabsOthpabsTblAutoLeftBase
{
}


[NDOPersistent]
public abstract class AgrDirnOwnpabsOthpabsTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class AgrDirnOwnpabsOthpabsTblAutoRightDerived : AgrDirnOwnpabsOthpabsTblAutoRightBase
{
}


[NDOPersistent]
public abstract class AgrBin1OwnpabsOthpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBin1OwnpabsOthpabsTblAutoRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1OwnpabsOthpabsTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrBin1OwnpabsOthpabsTblAutoLeftDerived : AgrBin1OwnpabsOthpabsTblAutoLeftBase
{
}


[NDOPersistent]
public abstract class AgrBin1OwnpabsOthpabsTblAutoRightBase
{
	int dummy;
	[NDORelation]
	AgrBin1OwnpabsOthpabsTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1OwnpabsOthpabsTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBin1OwnpabsOthpabsTblAutoRightDerived : AgrBin1OwnpabsOthpabsTblAutoRightBase
{
}


[NDOPersistent]
public abstract class AgrBi1nOwnpabsOthpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation]
	AgrBi1nOwnpabsOthpabsTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nOwnpabsOthpabsTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nOwnpabsOthpabsTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class AgrBi1nOwnpabsOthpabsTblAutoLeftDerived : AgrBi1nOwnpabsOthpabsTblAutoLeftBase
{
}


[NDOPersistent]
public abstract class AgrBi1nOwnpabsOthpabsTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBi1nOwnpabsOthpabsTblAutoLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBi1nOwnpabsOthpabsTblAutoRightDerived : AgrBi1nOwnpabsOthpabsTblAutoRightBase
{
}


[NDOPersistent]
public abstract class AgrBinnOwnpabsOthpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBinnOwnpabsOthpabsTblAutoRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBinnOwnpabsOthpabsTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class AgrBinnOwnpabsOthpabsTblAutoLeftDerived : AgrBinnOwnpabsOthpabsTblAutoLeftBase
{
}


[NDOPersistent]
public abstract class AgrBinnOwnpabsOthpabsTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBinnOwnpabsOthpabsTblAutoLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class AgrBinnOwnpabsOthpabsTblAutoRightDerived : AgrBinnOwnpabsOthpabsTblAutoRightBase
{
}


[NDOPersistent]
public abstract class CmpDir1OwnpabsOthpabsNoTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpDir1OwnpabsOthpabsNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OwnpabsOthpabsNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OwnpabsOthpabsNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpDir1OwnpabsOthpabsNoTblAutoLeftDerived : CmpDir1OwnpabsOthpabsNoTblAutoLeftBase
{
}


[NDOPersistent]
public abstract class CmpDir1OwnpabsOthpabsNoTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpDir1OwnpabsOthpabsNoTblAutoRightDerived : CmpDir1OwnpabsOthpabsNoTblAutoRightBase
{
}


[NDOPersistent]
public abstract class CmpDir1OwnpabsOthpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpDir1OwnpabsOthpabsTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OwnpabsOthpabsTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OwnpabsOthpabsTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpDir1OwnpabsOthpabsTblAutoLeftDerived : CmpDir1OwnpabsOthpabsTblAutoLeftBase
{
}


[NDOPersistent]
public abstract class CmpDir1OwnpabsOthpabsTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpDir1OwnpabsOthpabsTblAutoRightDerived : CmpDir1OwnpabsOthpabsTblAutoRightBase
{
}


[NDOPersistent]
public abstract class CmpBi11OwnpabsOthpabsNoTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi11OwnpabsOthpabsNoTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpabsOthpabsNoTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OwnpabsOthpabsNoTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi11OwnpabsOthpabsNoTblAutoLeftDerived : CmpBi11OwnpabsOthpabsNoTblAutoLeftBase
{
}


[NDOPersistent]
public abstract class CmpBi11OwnpabsOthpabsNoTblAutoRightBase
{
	int dummy;
	[NDORelation]
	CmpBi11OwnpabsOthpabsNoTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpabsOthpabsNoTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi11OwnpabsOthpabsNoTblAutoRightDerived : CmpBi11OwnpabsOthpabsNoTblAutoRightBase
{
}


[NDOPersistent]
public abstract class CmpBi11OwnpabsOthpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi11OwnpabsOthpabsTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpabsOthpabsTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OwnpabsOthpabsTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi11OwnpabsOthpabsTblAutoLeftDerived : CmpBi11OwnpabsOthpabsTblAutoLeftBase
{
}


[NDOPersistent]
public abstract class CmpBi11OwnpabsOthpabsTblAutoRightBase
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBi11OwnpabsOthpabsTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpabsOthpabsTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi11OwnpabsOthpabsTblAutoRightDerived : CmpBi11OwnpabsOthpabsTblAutoRightBase
{
}


[NDOPersistent]
public abstract class CmpDirnOwnpabsOthpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpDirnOwnpabsOthpabsTblAutoRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnOwnpabsOthpabsTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpDirnOwnpabsOthpabsTblAutoLeftDerived : CmpDirnOwnpabsOthpabsTblAutoLeftBase
{
}


[NDOPersistent]
public abstract class CmpDirnOwnpabsOthpabsTblAutoRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent]
public class CmpDirnOwnpabsOthpabsTblAutoRightDerived : CmpDirnOwnpabsOthpabsTblAutoRightBase
{
}


[NDOPersistent]
public abstract class CmpBin1OwnpabsOthpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBin1OwnpabsOthpabsTblAutoRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1OwnpabsOthpabsTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpBin1OwnpabsOthpabsTblAutoLeftDerived : CmpBin1OwnpabsOthpabsTblAutoLeftBase
{
}


[NDOPersistent]
public abstract class CmpBin1OwnpabsOthpabsTblAutoRightBase
{
	int dummy;
	[NDORelation]
	CmpBin1OwnpabsOthpabsTblAutoLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1OwnpabsOthpabsTblAutoLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBin1OwnpabsOthpabsTblAutoRightDerived : CmpBin1OwnpabsOthpabsTblAutoRightBase
{
}


[NDOPersistent]
public abstract class CmpBi1nOwnpabsOthpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi1nOwnpabsOthpabsTblAutoRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nOwnpabsOthpabsTblAutoRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nOwnpabsOthpabsTblAutoRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent]
public class CmpBi1nOwnpabsOthpabsTblAutoLeftDerived : CmpBi1nOwnpabsOthpabsTblAutoLeftBase
{
}


[NDOPersistent]
public abstract class CmpBi1nOwnpabsOthpabsTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBi1nOwnpabsOthpabsTblAutoLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBi1nOwnpabsOthpabsTblAutoRightDerived : CmpBi1nOwnpabsOthpabsTblAutoRightBase
{
}


[NDOPersistent]
public abstract class CmpBinnOwnpabsOthpabsTblAutoLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBinnOwnpabsOthpabsTblAutoRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBinnOwnpabsOthpabsTblAutoRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent]
public class CmpBinnOwnpabsOthpabsTblAutoLeftDerived : CmpBinnOwnpabsOthpabsTblAutoLeftBase
{
}


[NDOPersistent]
public abstract class CmpBinnOwnpabsOthpabsTblAutoRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBinnOwnpabsOthpabsTblAutoLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent]
public class CmpBinnOwnpabsOthpabsTblAutoRightDerived : CmpBinnOwnpabsOthpabsTblAutoRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrDir1OwnpabsNoTblGuidLeftBase
{
	int dummy;
	[NDORelation]
	AgrDir1OwnpabsNoTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OwnpabsNoTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OwnpabsNoTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpabsNoTblGuidLeftDerived : AgrDir1OwnpabsNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpabsNoTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrDir1OwnpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrDir1OwnpabsTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OwnpabsTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OwnpabsTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpabsTblGuidLeftDerived : AgrDir1OwnpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpabsTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBi11OwnpabsNoTblGuidLeftBase
{
	int dummy;
	[NDORelation]
	AgrBi11OwnpabsNoTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpabsNoTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OwnpabsNoTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpabsNoTblGuidLeftDerived : AgrBi11OwnpabsNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpabsNoTblGuidRight
{
	int dummy;
	[NDORelation]
	AgrBi11OwnpabsNoTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpabsNoTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBi11OwnpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OwnpabsTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpabsTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OwnpabsTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpabsTblGuidLeftDerived : AgrBi11OwnpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpabsTblGuidRight
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OwnpabsTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpabsTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrDirnOwnpabsNoTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrDirnOwnpabsNoTblGuidRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnOwnpabsNoTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOwnpabsNoTblGuidLeftDerived : AgrDirnOwnpabsNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOwnpabsNoTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrDirnOwnpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrDirnOwnpabsTblGuidRight)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnOwnpabsTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOwnpabsTblGuidLeftDerived : AgrDirnOwnpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOwnpabsTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBin1OwnpabsNoTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBin1OwnpabsNoTblGuidRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1OwnpabsNoTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OwnpabsNoTblGuidLeftDerived : AgrBin1OwnpabsNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OwnpabsNoTblGuidRight
{
	int dummy;
	[NDORelation]
	AgrBin1OwnpabsNoTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1OwnpabsNoTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBin1OwnpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBin1OwnpabsTblGuidRight)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1OwnpabsTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OwnpabsTblGuidLeftDerived : AgrBin1OwnpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OwnpabsTblGuidRight
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBin1OwnpabsTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1OwnpabsTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBi1nOwnpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation]
	AgrBi1nOwnpabsTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nOwnpabsTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nOwnpabsTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOwnpabsTblGuidLeftDerived : AgrBi1nOwnpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOwnpabsTblGuidRight
{
	int dummy;
	[NDORelation(typeof(AgrBi1nOwnpabsTblGuidLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBinnOwnpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBinnOwnpabsTblGuidRight))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBinnOwnpabsTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnOwnpabsTblGuidLeftDerived : AgrBinnOwnpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnOwnpabsTblGuidRight
{
	int dummy;
	[NDORelation(typeof(AgrBinnOwnpabsTblGuidLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpDir1OwnpabsNoTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpDir1OwnpabsNoTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OwnpabsNoTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OwnpabsNoTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpabsNoTblGuidLeftDerived : CmpDir1OwnpabsNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpabsNoTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpDir1OwnpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpDir1OwnpabsTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OwnpabsTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OwnpabsTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpabsTblGuidLeftDerived : CmpDir1OwnpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpabsTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBi11OwnpabsNoTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi11OwnpabsNoTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpabsNoTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OwnpabsNoTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpabsNoTblGuidLeftDerived : CmpBi11OwnpabsNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpabsNoTblGuidRight
{
	int dummy;
	[NDORelation]
	CmpBi11OwnpabsNoTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpabsNoTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBi11OwnpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi11OwnpabsTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpabsTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OwnpabsTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpabsTblGuidLeftDerived : CmpBi11OwnpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpabsTblGuidRight
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBi11OwnpabsTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpabsTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpDirnOwnpabsNoTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpDirnOwnpabsNoTblGuidRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnOwnpabsNoTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOwnpabsNoTblGuidLeftDerived : CmpDirnOwnpabsNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOwnpabsNoTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpDirnOwnpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpDirnOwnpabsTblGuidRight), RelationInfo.Composite), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnOwnpabsTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOwnpabsTblGuidLeftDerived : CmpDirnOwnpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOwnpabsTblGuidRight
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBin1OwnpabsNoTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBin1OwnpabsNoTblGuidRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1OwnpabsNoTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OwnpabsNoTblGuidLeftDerived : CmpBin1OwnpabsNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OwnpabsNoTblGuidRight
{
	int dummy;
	[NDORelation]
	CmpBin1OwnpabsNoTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1OwnpabsNoTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBin1OwnpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBin1OwnpabsTblGuidRight), RelationInfo.Composite), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1OwnpabsTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OwnpabsTblGuidLeftDerived : CmpBin1OwnpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OwnpabsTblGuidRight
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBin1OwnpabsTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1OwnpabsTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBi1nOwnpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi1nOwnpabsTblGuidRight relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nOwnpabsTblGuidRight RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nOwnpabsTblGuidRight relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOwnpabsTblGuidLeftDerived : CmpBi1nOwnpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOwnpabsTblGuidRight
{
	int dummy;
	[NDORelation(typeof(CmpBi1nOwnpabsTblGuidLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBinnOwnpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBinnOwnpabsTblGuidRight), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBinnOwnpabsTblGuidRight relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnOwnpabsTblGuidLeftDerived : CmpBinnOwnpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnOwnpabsTblGuidRight
{
	int dummy;
	[NDORelation(typeof(CmpBinnOwnpabsTblGuidLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OthpabsNoTblGuidLeft
{
	int dummy;
	[NDORelation]
	AgrDir1OthpabsNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OthpabsNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OthpabsNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrDir1OthpabsNoTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OthpabsNoTblGuidRightDerived : AgrDir1OthpabsNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OthpabsTblGuidLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrDir1OthpabsTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OthpabsTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OthpabsTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrDir1OthpabsTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OthpabsTblGuidRightDerived : AgrDir1OthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OthpabsNoTblGuidLeft
{
	int dummy;
	[NDORelation]
	AgrBi11OthpabsNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OthpabsNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OthpabsNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBi11OthpabsNoTblGuidRightBase
{
	int dummy;
	[NDORelation]
	AgrBi11OthpabsNoTblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OthpabsNoTblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OthpabsNoTblGuidRightDerived : AgrBi11OthpabsNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OthpabsTblGuidLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OthpabsTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OthpabsTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OthpabsTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBi11OthpabsTblGuidRightBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OthpabsTblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OthpabsTblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OthpabsTblGuidRightDerived : AgrBi11OthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOthpabsTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(AgrDirnOthpabsTblGuidRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnOthpabsTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrDirnOthpabsTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOthpabsTblGuidRightDerived : AgrDirnOthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OthpabsTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(AgrBin1OthpabsTblGuidRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1OthpabsTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBin1OthpabsTblGuidRightBase
{
	int dummy;
	[NDORelation]
	AgrBin1OthpabsTblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1OthpabsTblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OthpabsTblGuidRightDerived : AgrBin1OthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOthpabsNoTblGuidLeft
{
	int dummy;
	[NDORelation]
	AgrBi1nOthpabsNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nOthpabsNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nOthpabsNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBi1nOthpabsNoTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBi1nOthpabsNoTblGuidLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOthpabsNoTblGuidRightDerived : AgrBi1nOthpabsNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOthpabsTblGuidLeft
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi1nOthpabsTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nOthpabsTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nOthpabsTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBi1nOthpabsTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBi1nOthpabsTblGuidLeft)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOthpabsTblGuidRightDerived : AgrBi1nOthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnOthpabsTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(AgrBinnOthpabsTblGuidRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBinnOthpabsTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBinnOthpabsTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBinnOthpabsTblGuidLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnOthpabsTblGuidRightDerived : AgrBinnOthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OthpabsNoTblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpDir1OthpabsNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OthpabsNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OthpabsNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpDir1OthpabsNoTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OthpabsNoTblGuidRightDerived : CmpDir1OthpabsNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OthpabsTblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpDir1OthpabsTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OthpabsTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OthpabsTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpDir1OthpabsTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OthpabsTblGuidRightDerived : CmpDir1OthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OthpabsNoTblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi11OthpabsNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OthpabsNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OthpabsNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBi11OthpabsNoTblGuidRightBase
{
	int dummy;
	[NDORelation]
	CmpBi11OthpabsNoTblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OthpabsNoTblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OthpabsNoTblGuidRightDerived : CmpBi11OthpabsNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OthpabsTblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi11OthpabsTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OthpabsTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OthpabsTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBi11OthpabsTblGuidRightBase
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBi11OthpabsTblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OthpabsTblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OthpabsTblGuidRightDerived : CmpBi11OthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOthpabsTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(CmpDirnOthpabsTblGuidRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnOthpabsTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpDirnOthpabsTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOthpabsTblGuidRightDerived : CmpDirnOthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OthpabsTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(CmpBin1OthpabsTblGuidRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1OthpabsTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBin1OthpabsTblGuidRightBase
{
	int dummy;
	[NDORelation]
	CmpBin1OthpabsTblGuidLeft relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1OthpabsTblGuidLeft RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OthpabsTblGuidRightDerived : CmpBin1OthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOthpabsNoTblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi1nOthpabsNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nOthpabsNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nOthpabsNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBi1nOthpabsNoTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBi1nOthpabsNoTblGuidLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOthpabsNoTblGuidRightDerived : CmpBi1nOthpabsNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOthpabsTblGuidLeft
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi1nOthpabsTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nOthpabsTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nOthpabsTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBi1nOthpabsTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBi1nOthpabsTblGuidLeft)), MappingTable]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOthpabsTblGuidRightDerived : CmpBi1nOthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnOthpabsTblGuidLeft
{
	int dummy;
	[NDORelation(typeof(CmpBinnOthpabsTblGuidRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBinnOthpabsTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBinnOthpabsTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBinnOthpabsTblGuidLeft))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnOthpabsTblGuidRightDerived : CmpBinnOthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrDir1OwnpabsOthpabsNoTblGuidLeftBase
{
	int dummy;
	[NDORelation]
	AgrDir1OwnpabsOthpabsNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OwnpabsOthpabsNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OwnpabsOthpabsNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpabsOthpabsNoTblGuidLeftDerived : AgrDir1OwnpabsOthpabsNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrDir1OwnpabsOthpabsNoTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpabsOthpabsNoTblGuidRightDerived : AgrDir1OwnpabsOthpabsNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrDir1OwnpabsOthpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrDir1OwnpabsOthpabsTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrDir1OwnpabsOthpabsTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDir1OwnpabsOthpabsTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpabsOthpabsTblGuidLeftDerived : AgrDir1OwnpabsOthpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrDir1OwnpabsOthpabsTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDir1OwnpabsOthpabsTblGuidRightDerived : AgrDir1OwnpabsOthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBi11OwnpabsOthpabsNoTblGuidLeftBase
{
	int dummy;
	[NDORelation]
	AgrBi11OwnpabsOthpabsNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpabsOthpabsNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OwnpabsOthpabsNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpabsOthpabsNoTblGuidLeftDerived : AgrBi11OwnpabsOthpabsNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBi11OwnpabsOthpabsNoTblGuidRightBase
{
	int dummy;
	[NDORelation]
	AgrBi11OwnpabsOthpabsNoTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpabsOthpabsNoTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpabsOthpabsNoTblGuidRightDerived : AgrBi11OwnpabsOthpabsNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBi11OwnpabsOthpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OwnpabsOthpabsTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpabsOthpabsTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi11OwnpabsOthpabsTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpabsOthpabsTblGuidLeftDerived : AgrBi11OwnpabsOthpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBi11OwnpabsOthpabsTblGuidRightBase
{
	int dummy;
	[NDORelation, MappingTable]
	AgrBi11OwnpabsOthpabsTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi11OwnpabsOthpabsTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi11OwnpabsOthpabsTblGuidRightDerived : AgrBi11OwnpabsOthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrDirnOwnpabsOthpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrDirnOwnpabsOthpabsTblGuidRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrDirnOwnpabsOthpabsTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOwnpabsOthpabsTblGuidLeftDerived : AgrDirnOwnpabsOthpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrDirnOwnpabsOthpabsTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrDirnOwnpabsOthpabsTblGuidRightDerived : AgrDirnOwnpabsOthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBin1OwnpabsOthpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBin1OwnpabsOthpabsTblGuidRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBin1OwnpabsOthpabsTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OwnpabsOthpabsTblGuidLeftDerived : AgrBin1OwnpabsOthpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBin1OwnpabsOthpabsTblGuidRightBase
{
	int dummy;
	[NDORelation]
	AgrBin1OwnpabsOthpabsTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBin1OwnpabsOthpabsTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBin1OwnpabsOthpabsTblGuidRightDerived : AgrBin1OwnpabsOthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBi1nOwnpabsOthpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation]
	AgrBi1nOwnpabsOthpabsTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public AgrBi1nOwnpabsOthpabsTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBi1nOwnpabsOthpabsTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOwnpabsOthpabsTblGuidLeftDerived : AgrBi1nOwnpabsOthpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBi1nOwnpabsOthpabsTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBi1nOwnpabsOthpabsTblGuidLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBi1nOwnpabsOthpabsTblGuidRightDerived : AgrBi1nOwnpabsOthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBinnOwnpabsOthpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(AgrBinnOwnpabsOthpabsTblGuidRightBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(AgrBinnOwnpabsOthpabsTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnOwnpabsOthpabsTblGuidLeftDerived : AgrBinnOwnpabsOthpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class AgrBinnOwnpabsOthpabsTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(AgrBinnOwnpabsOthpabsTblGuidLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class AgrBinnOwnpabsOthpabsTblGuidRightDerived : AgrBinnOwnpabsOthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpDir1OwnpabsOthpabsNoTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpDir1OwnpabsOthpabsNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OwnpabsOthpabsNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OwnpabsOthpabsNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpabsOthpabsNoTblGuidLeftDerived : CmpDir1OwnpabsOthpabsNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpDir1OwnpabsOthpabsNoTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpabsOthpabsNoTblGuidRightDerived : CmpDir1OwnpabsOthpabsNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpDir1OwnpabsOthpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpDir1OwnpabsOthpabsTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpDir1OwnpabsOthpabsTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDir1OwnpabsOthpabsTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpabsOthpabsTblGuidLeftDerived : CmpDir1OwnpabsOthpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpDir1OwnpabsOthpabsTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDir1OwnpabsOthpabsTblGuidRightDerived : CmpDir1OwnpabsOthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBi11OwnpabsOthpabsNoTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi11OwnpabsOthpabsNoTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpabsOthpabsNoTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OwnpabsOthpabsNoTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpabsOthpabsNoTblGuidLeftDerived : CmpBi11OwnpabsOthpabsNoTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBi11OwnpabsOthpabsNoTblGuidRightBase
{
	int dummy;
	[NDORelation]
	CmpBi11OwnpabsOthpabsNoTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpabsOthpabsNoTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpabsOthpabsNoTblGuidRightDerived : CmpBi11OwnpabsOthpabsNoTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBi11OwnpabsOthpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite), MappingTable]
	CmpBi11OwnpabsOthpabsTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpabsOthpabsTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi11OwnpabsOthpabsTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpabsOthpabsTblGuidLeftDerived : CmpBi11OwnpabsOthpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBi11OwnpabsOthpabsTblGuidRightBase
{
	int dummy;
	[NDORelation, MappingTable]
	CmpBi11OwnpabsOthpabsTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi11OwnpabsOthpabsTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi11OwnpabsOthpabsTblGuidRightDerived : CmpBi11OwnpabsOthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpDirnOwnpabsOthpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpDirnOwnpabsOthpabsTblGuidRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpDirnOwnpabsOthpabsTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOwnpabsOthpabsTblGuidLeftDerived : CmpDirnOwnpabsOthpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpDirnOwnpabsOthpabsTblGuidRightBase
{
	int dummy;
	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpDirnOwnpabsOthpabsTblGuidRightDerived : CmpDirnOwnpabsOthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBin1OwnpabsOthpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBin1OwnpabsOthpabsTblGuidRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBin1OwnpabsOthpabsTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OwnpabsOthpabsTblGuidLeftDerived : CmpBin1OwnpabsOthpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBin1OwnpabsOthpabsTblGuidRightBase
{
	int dummy;
	[NDORelation]
	CmpBin1OwnpabsOthpabsTblGuidLeftBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBin1OwnpabsOthpabsTblGuidLeftBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBin1OwnpabsOthpabsTblGuidRightDerived : CmpBin1OwnpabsOthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBi1nOwnpabsOthpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(RelationInfo.Composite)]
	CmpBi1nOwnpabsOthpabsTblGuidRightBase relField;

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public CmpBi1nOwnpabsOthpabsTblGuidRightBase RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBi1nOwnpabsOthpabsTblGuidRightBase relObj)
	{
		relField = relObj;
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOwnpabsOthpabsTblGuidLeftDerived : CmpBi1nOwnpabsOthpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBi1nOwnpabsOthpabsTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBi1nOwnpabsOthpabsTblGuidLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBi1nOwnpabsOthpabsTblGuidRightDerived : CmpBi1nOwnpabsOthpabsTblGuidRightBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBinnOwnpabsOthpabsTblGuidLeftBase
{
	int dummy;
	[NDORelation(typeof(CmpBinnOwnpabsOthpabsTblGuidRightBase), RelationInfo.Composite)]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
	public void AssignRelation(CmpBinnOwnpabsOthpabsTblGuidRightBase relObj)
	{
		relField.Add(relObj);
	}
	public void RemoveRelatedObject()
	{
		relField.RemoveAt(0);
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnOwnpabsOthpabsTblGuidLeftDerived : CmpBinnOwnpabsOthpabsTblGuidLeftBase
{
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public abstract class CmpBinnOwnpabsOthpabsTblGuidRightBase
{
	int dummy;
	[NDORelation(typeof(CmpBinnOwnpabsOthpabsTblGuidLeftBase))]
	IList relField = new ArrayList();

	public int Dummy
	{
		get { return dummy; }
		set { dummy = value; }
	}
	public IList RelField
	{
		get { return relField; }
		set { relField = value; }
	}
}


[NDOPersistent, NDOOidType(typeof(Guid))]
public class CmpBinnOwnpabsOthpabsTblGuidRightDerived : CmpBinnOwnpabsOthpabsTblGuidRightBase
{
}


}
