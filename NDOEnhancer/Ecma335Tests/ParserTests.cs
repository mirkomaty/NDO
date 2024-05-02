using NDOEnhancer.Ecma335;
using NDOEnhancer.Ecma335.Bytes;
using NUnit.Framework;

namespace Ecma335Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void ParseIds()
        {
            var ids = new[]{ "A", "Test", "$Test", "@Foo?", "?_X_ MyType`1", "'Weird Identifier'", @"'Odd\102Char'", @"'Embedded\nReturn'" };
            foreach (var id in ids)
            {
                EcmaDottedName dottedName = new EcmaDottedName();
                Assert.That( dottedName.ParseId( id ), $"{id} should be a valid id" );
            }
        }

        [Test]
        public void ParseDottedNames()
        {
            var ids = new[]{ @"System.Console", "'My Project'.'My Component'.'My Name'", "System.IComparable`1" };
            foreach (var id in ids)
            {
                EcmaDottedName dottedName = new EcmaDottedName();
                Assert.That( dottedName.Parse( id ), $"{id} should be a valid id" );
            }
        }

        [Test]
        public void ParseSlashedNames()
        {
            var ids = new[]{ @"System.Console", "'My Project'.'My Component'.'My Name'", "System.IComparable`1" };
            EcmaDottedName dottedName = new EcmaDottedName();
            foreach (var id in ids)
            {
                var combinedIds = new[]{ $"test/{id}", $"'te st'/{id}", $"{id}/test", $"{id}/'te st'" };
                foreach (var id2 in combinedIds)
                {
                    Assert.That( dottedName.Parse( id2 ), $"{id2} should be a valid id" );
                }
            }
        }

        [Test]
        public void ParseCustom1()
        {
            var custom = @".custom instance void myAttribute::.ctor(bool, bool) = ( 01 00 00 01 00 00 )";
            EcmaCustomAttrDecl decl = new EcmaCustomAttrDecl();
            Assert.That( decl.Parse(custom) );
            Assert.That( decl.Content.Equals( custom ) );
            Assert.That( decl.TypeName.Equals( "myAttribute" ) );
            Assert.That( decl.Bytes.Length.Equals( 6 ) );

            custom = ".custom instance void [System.Runtime]System.CLSCompliantAttribute::.ctor(bool) = ( 01 00 01 00 00 )";
            decl = new EcmaCustomAttrDecl();
            Assert.That( decl.Parse( custom ) );
            Assert.That( decl.Content.Equals( custom ) );
            Assert.That( decl.TypeName.Equals( "System.CLSCompliantAttribute" ) );
            Assert.That( decl.ResolutionScope == "[System.Runtime]" );
            Assert.That( decl.Bytes.Length.Equals( 5 ) );

            custom = ".custom instance void [System.Runtime]System.AttributeUsageAttribute::.ctor(valuetype [System.Runtime]System.AttributeTargets) = ( 01 00 01 00 00 00 02 00 54 02 0D 41 6C 6C 6F 77 4D 75 6C 74 69 70 6C 65 01 54 02 09 49 6E 68 65 72 69 74 65 64 01 )";
        }

        [Test]
        public void ParseEcmaBytes()
        {
            var custom = @".custom instance void .ctor(bool, bool) = ( 01 00 00 01 00 00 )";
            EcmaCustomAttrDecl decl = new EcmaCustomAttrDecl();
            Assert.That( decl.Parse( custom ) );
            var bytes = decl.Bytes;
            var types = new[]{ typeof(bool), typeof(bool) };

            EcmaBytes ecmaBytes = new EcmaBytes();
            ecmaBytes.Parse( bytes, types, out var ctorValues, out var _ );
            var ctorValuesArray = ctorValues.ToArray();
            Assert.That( !(bool) ctorValuesArray[0]! );
            Assert.That( (bool) ctorValuesArray[1]! );

            // Test char
            bytes = new byte[] { 1, 0, 65, 0, 0, 0 };
            ecmaBytes = new EcmaBytes();
            types = new[] { typeof( char ) };
            ecmaBytes.Parse( bytes, types, out ctorValues, out _ );

            Assert.That( ( (char) ctorValues.First()! ).Equals( 'A' ) );

            // Int64
            bytes = new byte[] { 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            ecmaBytes = new EcmaBytes();
            types = new[] { typeof( Int64 ) };
            ecmaBytes.Parse( bytes, types, out ctorValues, out _ );

            Assert.That( ( (Int64) ctorValues.First()! ).Equals( 1 ) );

            // Should throw, because of too less bytes
            bytes = new byte[] { 1, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
            ecmaBytes = new EcmaBytes();
            types = new[] { typeof( Int64 ) };
            bool thrown = false;
            try
            {
                ecmaBytes.Parse( bytes, types, out ctorValues, out _ );
            }
            catch (Exception ex)
            {
                thrown = true;
            }

            Assert.That( thrown );

            // Should throw, because of too less bytes for a string
            bytes = new byte[] { 1, 0 };
            ecmaBytes = new EcmaBytes();
            types = new[] { typeof( string ) };
            thrown = false;
            try
            {
                ecmaBytes.Parse( bytes, types, out ctorValues, out _ );
            }
            catch (Exception ex)
            {
                thrown = true;
            }

            Assert.That( thrown );

            // Should throw, because of too less bytes for a bool
            bytes = new byte[] { 1, 0, 0, 0 }; // only prolog and numNamed
            ecmaBytes = new EcmaBytes();
            types = new[] { typeof( bool ) };
            thrown = false;
            try
            {
                ecmaBytes.Parse( bytes, types, out ctorValues, out _ );
            }
            catch (Exception ex)
            {
                thrown = true;
            }

            Assert.That( thrown );
        }

        Type? GetNetType(string ilTypeName)
        {
            string tn = ilTypeName;
            if (tn.StartsWith( "valuetype " ))
                tn = tn.Substring( 10 );
            else if (tn.StartsWith( "class " ))
                tn = tn.Substring( 6 );
            tn = tn.Trim();

            var typeSpec = new EcmaTypeSpec();
            typeSpec.Parse( tn );
            var assyName = typeSpec.ResolutionScope.Substring(1, typeSpec.ResolutionScope.Length - 2);

            var dottedName = new EcmaDottedName();
            dottedName.Parse( typeSpec.TypenameWithoutScope );
            var typeName = dottedName.UnquotedName;
            return Type.GetType( typeName + ", " + assyName );
        }

        [Test]
        public void ParseAttributeUsage()
        {
            string custom = @".custom instance void [System.Runtime]System.AttributeUsageAttribute::.ctor(valuetype [System.Runtime]System.AttributeTargets) = ( 01 00 01 00 00 00 02 00 54 02 0D 41 6C 6C 6F 77 4D 75 6C 74 69 70 6C 65 01 54 02 09 49 6E 68 65 72 69 74 65 64 01 )";
            EcmaCustomAttrDecl decl = new EcmaCustomAttrDecl();
            Assert.That( decl.Parse( custom ) );
            var parameters = decl.ParameterList;
            Type? t = GetNetType(parameters);
            if (t == null)
                throw new Exception( "t is null" );
            var ecmaBytes = new EcmaBytes();
            var success = ecmaBytes.Parse( decl.Bytes, new[] { t }, out var ctorValues, out var _ );
            Assert.That( success );
        }
    }
}
