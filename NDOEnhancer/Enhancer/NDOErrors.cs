//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.com.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;

namespace NDOEnhancer
{
	internal class NDOErrors
	{
		//Sorry, the community version enhancer can't work with this NDO.dll.
		public const string WrongDll = "618FCF4C40663542690EF407377ACD29A427AC417899E5BA74E3678D7AA27B29A6B9AFEA537E26F223D2537FC89EABFBB6D524000665AF282394AA210698C7A3294991CC1D6C4BEAED84358D4160B822";
		//Sorry, there are too much classes for the community version of NDO.
		public const string TooMuchClasses = "FBBCA830CF12CBB2607857325401014F5E9B5570D8174E3DAA095A503E958479CC43157AB2146BA16DF8BB141CA605D21CBB2CF069E5E9A5A3506CF8366239298165AD0CC0758CB7B31B5B7842D0AA3F";
		//Sorry, there are too much fields for the community version of NDO.
		public const string TooMuchFields = "FBBCA830CF12CBB2607857325401014FD76A591848C192A70AC1D6C0C378921F74EE2E38EC6DC683D3F3C192FF24F1F432B97EB332F02149BE2A57A98F5338C9D8C404937802DC6F556CE358A864105C";
		//Sorry, there are too much relations for the community version of NDO.
		public const string TooMuchRelations = "FBBCA830CF12CBB2607857325401014F33058804C1782D7FDAF1ABEB3DB976F7242F6C49C984A21E19ECF309F816A9B1F3DE373639CBB15FF45B61B5844D7F99C8723FB3A53D445FBB500302E4F8A310";
		//Sorry, there are too much objects for the community version of NDO.
		public const string TooMuchObjects = "FBBCA830CF12CBB2607857325401014F322D5F6A6B13DAEA36D106E0FAE7C0AC05C1B427852485E42F436D979F0DDEF80002D59316CF1E13A842FA6B40F7C53335C95E8C3C8F8C98596F1964B25DC693";
		//Too much elements in the mapping file for the community version of NDO.
		public const string TooMuchElements = "00CE2D0EBFB272453C3354C78505621D478ED35128D7DFB8C56708D9AAD8217005559B83ABB900B4951FA2856CC043558997EE7E1D1303727FC0E892E61D862D5CF3D82DA73BC41738CB40080187BE89";
	}
}
