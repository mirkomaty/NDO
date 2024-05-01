using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDOEnhancer.Enhancer.Ecma335
{
	internal class EcmaTypeSpec
	{
	}
}

/*

typeSpec : className
| '[' name1 ']'
| '[' '.module' name1 ']'
| type
;

className : '[' name1 ']' slashedName
| '[' '.module' name1 ']' slashedName
| slashedName
;

slashedName : name1
| slashedName '/' name1
;


name1 : id => empty
| DOTTEDNAME
| name1 '.' name1
;

slashedName: [id{/id}]id{.id}

typeSpec: ['[' ['.module'] id ']']  [id{/id}]id{.id}


 */