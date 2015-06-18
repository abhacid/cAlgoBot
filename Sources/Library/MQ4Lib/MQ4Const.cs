using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAlgo.MQ4
{
	public static class MQ4Const
	{
		public const bool True = true;
		public const bool False = false;
		public const bool TRUE = true;
		public const bool FALSE = false;
		//public Mq4Null NULL;
		public const int EMPTY = -1;
		public const double EMPTY_VALUE = 2147483647;
		public const int WHOLE_ARRAY = 0;

		public const int MODE_SMA = 0;				//Simple moving average
		public const int MODE_EMA = 1;				//Exponential moving average,
		public const int MODE_SMMA = 2;				//Smoothed moving average,
		public const int MODE_LWMA = 3;				//Linear weighted moving average.
		
		public const int PRICE_CLOSE = 0;			//Close price.
		public const int PRICE_OPEN = 1;			//Open price.
		public const int PRICE_HIGH = 2;			//High price.
		public const int PRICE_LOW = 3;				//Low price.
		public const int PRICE_MEDIAN = 4;			//Median price, (high+low)/2.
		public const int PRICE_TYPICAL = 5;			//Typical price, (high+low+close)/3.
		public const int PRICE_WEIGHTED = 6;		//Weighted close price, (high+low+close+close)/4.
		
		public const int DRAW_LINE = 0;
		public const int DRAW_SECTION = 1;
		public const int DRAW_HISTOGRAM = 2;
		public const int DRAW_ARROW = 3;
		public const int DRAW_ZIGZAG = 4;
		public const int DRAW_NONE = 12;

		public const int STYLE_SOLID = 0;
		public const int STYLE_DASH = 1;
		public const int STYLE_DOT = 2;
		public const int STYLE_DASHDOT = 3;
		public const int STYLE_DASHDOTDOT = 4;

		public const int MODE_OPEN = 0;
		public const int MODE_LOW = 1;
		public const int MODE_HIGH = 2;
		public const int MODE_CLOSE = 3;
		public const int MODE_VOLUME = 4;
		public const int MODE_TIME = 5;
		public const int MODE_BID = 9;
		public const int MODE_ASK = 10;
		public const int MODE_POINT = 11;
		public const int MODE_DIGITS = 12;
		public const int MODE_SPREAD = 13;
		public const int MODE_TRADEALLOWED = 22;
		public const int MODE_PROFITCALCMODE = 27;
		public const int MODE_MARGINCALCMODE = 28;
		public const int MODE_SWAPTYPE = 26;
		public const int MODE_TICKSIZE = 17;
		public const int MODE_FREEZELEVEL = 33;
		public const int MODE_STOPLEVEL = 14;
		/*public const int MODE_LOTSIZE = 15;
		public const int MODE_TICKVALUE = 16;
		public const int MODE_SWAPLONG = 18;
		public const int MODE_SWAPSHORT = 19;
		public const int MODE_STARTING = 20;
		public const int MODE_EXPIRATION = 21;
		public const int MODE_MINLOT = 23;
		public const int MODE_LOTSTEP = 24;
		public const int MODE_MAXLOT = 25;
		public const int MODE_MARGININIT = 29;
		public const int MODE_MARGINMAINTENANCE = 30;
		public const int MODE_MARGINHEDGED = 31;
		public const int MODE_MARGINREQUIRED = 32;*/

		public const int OBJ_VLINE = 0;
		public const int OBJ_HLINE = 1;
		public const int OBJ_TREND = 2;
		public const int OBJ_FIBO = 10;
		/*public const int OBJ_TRENDBYANGLE = 3;
		public const int OBJ_REGRESSION = 4;
		public const int OBJ_CHANNEL = 5;
		public const int OBJ_STDDEVCHANNEL = 6;
		public const int OBJ_GANNLINE = 7;
		public const int OBJ_GANNFAN = 8;
		public const int OBJ_GANNGRID = 9;
		public const int OBJ_FIBOTIMES = 11;
		public const int OBJ_FIBOFAN = 12;
		public const int OBJ_FIBOARC = 13;
		public const int OBJ_EXPANSION = 14;
		public const int OBJ_FIBOCHANNEL = 15;*/
		public const int OBJ_RECTANGLE = 16;
		/*public const int OBJ_TRIANGLE = 17;
		public const int OBJ_ELLIPSE = 18;
		public const int OBJ_PITCHFORK = 19;
		public const int OBJ_CYCLES = 20;*/
		public const int OBJ_TEXT = 21;
		public const int OBJ_ARROW = 22;
		public const int OBJ_LABEL = 23;

		public const int OBJPROP_TIME1 = 0;
		public const int OBJPROP_PRICE1 = 1;
		public const int OBJPROP_TIME2 = 2;
		public const int OBJPROP_PRICE2 = 3;
		public const int OBJPROP_TIME3 = 4;
		public const int OBJPROP_PRICE3 = 5;
		public const int OBJPROP_COLOR = 6;
		public const int OBJPROP_STYLE = 7;
		public const int OBJPROP_WIDTH = 8;
		public const int OBJPROP_BACK = 9;
		public const int OBJPROP_RAY = 10;
		public const int OBJPROP_ELLIPSE = 11;
		//public const int OBJPROP_SCALE = 12;
		public const int OBJPROP_ANGLE = 13;
		//angle for text rotation
		public const int OBJPROP_ARROWCODE = 14;
		public const int OBJPROP_TIMEFRAMES = 15;
		//public const int OBJPROP_DEVIATION = 16;
		public const int OBJPROP_FONTSIZE = 100;
		public const int OBJPROP_CORNER = 101;
		public const int OBJPROP_XDISTANCE = 102;
		public const int OBJPROP_YDISTANCE = 103;
		public const int OBJPROP_FIBOLEVELS = 200;
		public const int OBJPROP_LEVELCOLOR = 201;
		public const int OBJPROP_LEVELSTYLE = 202;
		public const int OBJPROP_LEVELWIDTH = 203;
		public const int OBJPROP_FIRSTLEVEL = 210;

		public const int PERIOD_M1 = 1;
		public const int PERIOD_M5 = 5;
		public const int PERIOD_M15 = 15;
		public const int PERIOD_M30 = 30;
		public const int PERIOD_H1 = 60;
		public const int PERIOD_H4 = 240;
		public const int PERIOD_D1 = 1440;
		public const int PERIOD_W1 = 10080;
		public const int PERIOD_MN1 = 43200;

		public const int TIME_DATE = 1;
		public const int TIME_MINUTES = 2;
		public const int TIME_SECONDS = 4;

		public const int MODE_MAIN = 0;
		public const int MODE_PLUSDI = 1;
		public const int MODE_MINUSDI = 2;
		public const int MODE_SIGNAL = 1;
		public const int MODE_UPPER = 1;
		public const int MODE_LOWER = 2;

		public const int CLR_NONE = 32768;

		public const int White = 16777215;
		public const int Snow = 16448255;
		public const int MintCream = 16449525;
		public const int LavenderBlush = 16118015;
		public const int AliceBlue = 16775408;
		public const int Honeydew = 15794160;
		public const int Ivory = 15794175;
		public const int Seashell = 15660543;
		public const int WhiteSmoke = 16119285;
		public const int OldLace = 15136253;
		public const int MistyRose = 14804223;
		public const int Lavender = 16443110;
		public const int Linen = 15134970;
		public const int LightCyan = 16777184;
		public const int LightYellow = 14745599;
		public const int Cornsilk = 14481663;
		public const int PapayaWhip = 14020607;
		public const int AntiqueWhite = 14150650;
		public const int Beige = 14480885;
		public const int LemonChiffon = 13499135;
		public const int BlanchedAlmond = 13495295;
		public const int LightGoldenrod = 13826810;
		public const int Bisque = 12903679;
		public const int Pink = 13353215;
		public const int PeachPuff = 12180223;
		public const int Gainsboro = 14474460;
		public const int LightPink = 12695295;
		public const int Moccasin = 11920639;
		public const int NavajoWhite = 11394815;
		public const int Wheat = 11788021;
		public const int LightGray = 13882323;
		public const int PaleTurquoise = 15658671;
		public const int PaleGoldenrod = 11200750;
		public const int PowderBlue = 15130800;
		public const int Thistle = 14204888;
		public const int PaleGreen = 10025880;
		public const int LightBlue = 15128749;
		public const int LightSteelBlue = 14599344;
		public const int LightSkyBlue = 16436871;
		public const int Silver = 12632256;
		public const int Aquamarine = 13959039;
		public const int LightGreen = 9498256;
		public const int Khaki = 9234160;
		public const int Plum = 14524637;
		public const int LightSalmon = 8036607;
		public const int SkyBlue = 15453831;
		public const int LightCoral = 8421616;
		public const int Violet = 15631086;
		public const int Salmon = 7504122;
		public const int HotPink = 11823615;
		public const int BurlyWood = 8894686;
		public const int DarkSalmon = 8034025;
		public const int Tan = 9221330;
		public const int MediumSlateBlue = 15624315;
		public const int SandyBrown = 6333684;
		public const int DarkGray = 11119017;
		public const int CornflowerBlue = 15570276;
		public const int Coral = 5275647;
		public const int PaleVioletRed = 9662683;
		public const int MediumPurple = 14381203;
		public const int Orchid = 14053594;
		public const int RosyBrown = 9408444;
		public const int Tomato = 4678655;
		public const int DarkSeaGreen = 9419919;
		public const int Cyan = 16776960;
		public const int MediumAquamarine = 11193702;
		public const int GreenYellow = 3145645;
		public const int MediumOrchid = 13850042;
		public const int IndianRed = 6053069;
		public const int DarkKhaki = 7059389;
		public const int SlateBlue = 13458026;
		public const int RoyalBlue = 14772545;
		public const int Turquoise = 13688896;
		public const int DodgerBlue = 16748574;
		public const int MediumTurquoise = 13422920;
		public const int DeepPink = 9639167;
		public const int LightSlateGray = 10061943;
		public const int BlueViolet = 14822282;
		public const int Peru = 4163021;
		public const int SlateGray = 9470064;
		public const int Gray = 8421504;
		public const int Red = 255;
		public const int Magenta = 16711935;
		public const int Blue = 16711680;
		public const int DeepSkyBlue = 16760576;
		public const int Aqua = 16776960;
		public const int SpringGreen = 8388352;
		public const int Lime = 65280;
		public const int Chartreuse = 65407;
		public const int Yellow = 65535;
		public const int Gold = 55295;
		public const int Orange = 42495;
		public const int DarkOrange = 36095;
		public const int OrangeRed = 17919;
		public const int LimeGreen = 3329330;
		public const int YellowGreen = 3329434;
		public const int DarkOrchid = 13382297;
		public const int CadetBlue = 10526303;
		public const int LawnGreen = 64636;
		public const int MediumSpringGreen = 10156544;
		public const int Goldenrod = 2139610;
		public const int SteelBlue = 11829830;
		public const int Crimson = 3937500;
		public const int Chocolate = 1993170;
		public const int MediumSeaGreen = 7451452;
		public const int MediumVioletRed = 8721863;
		public const int FireBrick = 2237106;
		public const int DarkViolet = 13828244;
		public const int LightSeaGreen = 11186720;
		public const int DimGray = 6908265;
		public const int DarkTurquoise = 13749760;
		public const int Brown = 2763429;
		public const int MediumBlue = 13434880;
		public const int Sienna = 2970272;
		public const int DarkSlateBlue = 9125192;
		public const int DarkGoldenrod = 755384;
		public const int SeaGreen = 5737262;
		public const int OliveDrab = 2330219;
		public const int ForestGreen = 2263842;
		public const int SaddleBrown = 1262987;
		public const int DarkOliveGreen = 3107669;
		public const int DarkBlue = 9109504;
		public const int MidnightBlue = 7346457;
		public const int Indigo = 8519755;
		public const int Maroon = 128;
		public const int Purple = 8388736;
		public const int Navy = 8388608;
		public const int Teal = 8421376;
		public const int Green = 32768;
		public const int Olive = 32896;
		public const int DarkSlateGray = 5197615;
		public const int DarkGreen = 25600;
		public const int Black = 0;

		public const int SYMBOL_LEFTPRICE = 5;
		public const int SYMBOL_RIGHTPRICE = 6;
		public const int SYMBOL_ARROWUP = 241;
		public const int SYMBOL_ARROWDOWN = 242;
		public const int SYMBOL_STOPSIGN = 251;
		/*public const int SYMBOL_THUMBSUP = 67;
		public const int SYMBOL_THUMBSDOWN = 68;
		public const int SYMBOL_CHECKSIGN = 25;*/

		public const int MODE_ASCEND = 1;
		public const int MODE_DESCEND = 2;
		public const int MODE_TENKANSEN = 1;
		public const int MODE_KIJUNSEN = 2;
		public const int MODE_SENKOUSPANA = 3;
		public const int MODE_SENKOUSPANB = 4;
		public const int MODE_CHINKOUSPAN = 5;

		public const int OP_BUY = 0;
		public const int OP_SELL = 1;
		public const int OP_BUYLIMIT = 2;
		public const int OP_SELLLIMIT = 3;
		public const int OP_BUYSTOP = 4;
		public const int OP_SELLSTOP = 5;

		public const int OBJ_PERIOD_M1 = 0x1;
		public const int OBJ_PERIOD_M5 = 0x2;
		public const int OBJ_PERIOD_M15 = 0x4;
		public const int OBJ_PERIOD_M30 = 0x8;
		public const int OBJ_PERIOD_H1 = 0x10;
		public const int OBJ_PERIOD_H4 = 0x20;
		public const int OBJ_PERIOD_D1 = 0x40;
		public const int OBJ_PERIOD_W1 = 0x80;
		public const int OBJ_PERIOD_MN1 = 0x100;
		public const int OBJ_ALL_PERIODS = 0x1ff;

		public const int REASON_REMOVE = 1;
		public const int REASON_RECOMPILE = 2;
		public const int REASON_CHARTCHANGE = 3;
		public const int REASON_CHARTCLOSE = 4;
		public const int REASON_PARAMETERS = 5;
		public const int REASON_ACCOUNT = 6;

		public const int ERR_NO_ERROR = 0;
		public const int ERR_NO_RESULT = 1;
		public const int ERR_COMMON_ERROR = 2;
		public const int ERR_INVALID_TRADE_PARAMETERS = 3;
		public const int ERR_SERVER_BUSY = 4;
		public const int ERR_OLD_VERSION = 5;
		public const int ERR_NO_CONNECTION = 6;
		public const int ERR_NOT_ENOUGH_RIGHTS = 7;
		public const int ERR_TOO_FREQUENT_REQUESTS = 8;
		public const int ERR_MALFUNCTIONAL_TRADE = 9;
		public const int ERR_ACCOUNT_DISABLED = 64;
		public const int ERR_INVALID_ACCOUNT = 65;
		public const int ERR_TRADE_TIMEOUT = 128;
		public const int ERR_INVALID_PRICE = 129;
		public const int ERR_INVALID_STOPS = 130;
		public const int ERR_INVALID_TRADE_VOLUME = 131;
		public const int ERR_MARKET_CLOSED = 132;
		public const int ERR_TRADE_DISABLED = 133;
		public const int ERR_NOT_ENOUGH_MONEY = 134;
		public const int ERR_PRICE_CHANGED = 135;
		public const int ERR_OFF_QUOTES = 136;
		public const int ERR_BROKER_BUSY = 137;
		public const int ERR_REQUOTE = 138;
		public const int ERR_ORDER_LOCKED = 139;
		public const int ERR_LONG_POSITIONS_ONLY_ALLOWED = 140;
		public const int ERR_TOO_MANY_REQUESTS = 141;
		public const int ERR_TRADE_MODIFY_DENIED = 145;
		public const int ERR_TRADE_CONTEXT_BUSY = 146;
		public const int ERR_TRADE_EXPIRATION_DENIED = 147;
		public const int ERR_TRADE_TOO_MANY_ORDERS = 148;
		public const int ERR_TRADE_HEDGE_PROHIBITED = 149;
		public const int ERR_TRADE_PROHIBITED_BY_FIFO = 150;
		public const int ERR_NO_MQLERROR = 4000;
		public const int ERR_WRONG_FUNCTION_POINTER = 4001;
		public const int ERR_ARRAY_INDEX_OUT_OF_RANGE = 4002;
		public const int ERR_NO_MEMORY_FOR_CALL_STACK = 4003;
		public const int ERR_RECURSIVE_STACK_OVERFLOW = 4004;
		public const int ERR_NOT_ENOUGH_STACK_FOR_PARAM = 4005;
		public const int ERR_NO_MEMORY_FOR_PARAM_STRING = 4006;
		public const int ERR_NO_MEMORY_FOR_TEMP_STRING = 4007;
		public const int ERR_NOT_INITIALIZED_STRING = 4008;
		public const int ERR_NOT_INITIALIZED_ARRAYSTRING = 4009;
		public const int ERR_NO_MEMORY_FOR_ARRAYSTRING = 4010;
		public const int ERR_TOO_LONG_STRING = 4011;
		public const int ERR_REMAINDER_FROM_ZERO_DIVIDE = 4012;
		public const int ERR_ZERO_DIVIDE = 4013;
		public const int ERR_UNKNOWN_COMMAND = 4014;
		public const int ERR_WRONG_JUMP = 4015;
		public const int ERR_NOT_INITIALIZED_ARRAY = 4016;
		public const int ERR_DLL_CALLS_NOT_ALLOWED = 4017;
		public const int ERR_CANNOT_LOAD_LIBRARY = 4018;
		public const int ERR_CANNOT_CALL_FUNCTION = 4019;
		public const int ERR_EXTERNAL_CALLS_NOT_ALLOWED = 4020;
		public const int ERR_NO_MEMORY_FOR_RETURNED_STR = 4021;
		public const int ERR_SYSTEM_BUSY = 4022;
		public const int ERR_INVALID_FUNCTION_PARAMSCNT = 4050;
		public const int ERR_INVALID_FUNCTION_PARAMVALUE = 4051;
		public const int ERR_STRING_FUNCTION_INTERNAL = 4052;
		public const int ERR_SOME_ARRAY_ERROR = 4053;
		public const int ERR_INCORRECT_SERIESARRAY_USING = 4054;
		public const int ERR_CUSTOM_INDICATOR_ERROR = 4055;
		public const int ERR_INCOMPATIBLE_ARRAYS = 4056;
		public const int ERR_GLOBAL_VARIABLES_PROCESSING = 4057;
		public const int ERR_GLOBAL_VARIABLE_NOT_FOUND = 4058;
		public const int ERR_FUNC_NOT_ALLOWED_IN_TESTING = 4059;
		public const int ERR_FUNCTION_NOT_CONFIRMED = 4060;
		public const int ERR_SEND_MAIL_ERROR = 4061;
		public const int ERR_STRING_PARAMETER_EXPECTED = 4062;
		public const int ERR_INTEGER_PARAMETER_EXPECTED = 4063;
		public const int ERR_DOUBLE_PARAMETER_EXPECTED = 4064;
		public const int ERR_ARRAY_AS_PARAMETER_EXPECTED = 4065;
		public const int ERR_HISTORY_WILL_UPDATED = 4066;
		public const int ERR_TRADE_ERROR = 4067;
		public const int ERR_END_OF_FILE = 4099;
		public const int ERR_SOME_FILE_ERROR = 4100;
		public const int ERR_WRONG_FILE_NAME = 4101;
		public const int ERR_TOO_MANY_OPENED_FILES = 4102;
		public const int ERR_CANNOT_OPEN_FILE = 4103;
		public const int ERR_INCOMPATIBLE_FILEACCESS = 4104;
		public const int ERR_NO_ORDER_SELECTED = 4105;
		public const int ERR_UNKNOWN_SYMBOL = 4106;
		public const int ERR_INVALID_PRICE_PARAM = 4107;
		public const int ERR_INVALID_TICKET = 4108;
		public const int ERR_TRADE_NOT_ALLOWED = 4109;
		public const int ERR_LONGS_NOT_ALLOWED = 4110;
		public const int ERR_SHORTS_NOT_ALLOWED = 4111;
		public const int ERR_OBJECT_ALREADY_EXISTS = 4200;
		public const int ERR_UNKNOWN_OBJECT_PROPERTY = 4201;
		public const int ERR_OBJECT_DOES_NOT_EXIST = 4202;
		public const int ERR_UNKNOWN_OBJECT_TYPE = 4203;
		public const int ERR_NO_OBJECT_NAME = 4204;
		public const int ERR_OBJECT_COORDINATES_ERROR = 4205;
		public const int ERR_NO_SPECIFIED_SUBWINDOW = 4206;
		public const int ERR_SOME_OBJECT_ERROR = 4207;

		public const string xArrow = "✖";


		public static string GetArrowByCode(int code)
		{
			switch (code)
			{
				case 0:
					return string.Empty;
				case 32:
					return " ";
				case 33:
					return "✏";
				case 34:
					return "✂";
				case 35:
					return "✁";
				case 40:
					return "☎";
				case 41:
					return "✆";
				case 42:
					return "✉";
				case 54:
					return "⌛";
				case 55:
					return "⌨";
				case 62:
					return "✇";
				case 63:
					return "✍";
				case 65:
					return "✌";
				case 69:
					return "☜";
				case 70:
					return "☞";
				case 71:
					return "☝";
				case 72:
					return "☟";
				case 74:
					return "☺";
				case 76:
					return "☹";
				case 78:
					return "☠";
				case 79:
					return "⚐";
				case 81:
					return "✈";
				case 82:
					return "☼";
				case 84:
					return "❄";
				case 86:
					return "✞";
				case 88:
					return "✠";
				case 89:
					return "✡";
				case 90:
					return "☪";
				case 91:
					return "☯";
				case 92:
					return "ॐ";
				case 93:
					return "☸";
				case 94:
					return "♈";
				case 95:
					return "♉";
				case 96:
					return "♊";
				case 97:
					return "♋";
				case 98:
					return "♌";
				case 99:
					return "♍";
				case 100:
					return "♎";
				case 101:
					return "♏";
				case 102:
					return "♐";
				case 103:
					return "♑";
				case 104:
					return "♒";
				case 105:
					return "♓";
				case 106:
					return "&";
				case 107:
					return "&";
				case 108:
					return "●";
				case 109:
					return "❍";
				case 110:
					return "■";
				case 111:
				case 112:
					return "□";
				case 113:
					return "❑";
				case 114:
					return "❒";
				case 115:
				case 116:
					return "⧫";
				case 117:
				case 119:
					return "◆";
				case 118:
					return "❖";
				case 120:
					return "⌧";
				case 121:
					return "⍓";
				case 122:
					return "⌘";
				case 123:
					return "❀";
				case 124:
					return "✿";
				case 125:
					return "❝";
				case 126:
					return "❞";
				case 127:
					return "▯";
				case 128:
					return "⓪";
				case 129:
					return "①";
				case 130:
					return "②";
				case 131:
					return "③";
				case 132:
					return "④";
				case 133:
					return "⑤";
				case 134:
					return "⑥";
				case 135:
					return "⑦";
				case 136:
					return "⑧";
				case 137:
					return "⑨";
				case 138:
					return "⑩";
				case 139:
					return "⓿";
				case 140:
					return "❶";
				case 141:
					return "❷";
				case 142:
					return "❸";
				case 143:
					return "❹";
				case 144:
					return "❺";
				case 145:
					return "❻";
				case 146:
					return "❼";
				case 147:
					return "❽";
				case 148:
					return "❾";
				case 149:
					return "❿";
				case 158:
					return "·";
				case 159:
					return "•";
				case 160:
				case 166:
					return "▪";
				case 161:
					return "○";
				case 162:
				case 164:
					return "⭕";
				case 165:
					return "◎";
				case 167:
					return "✖";
				case 168:
					return "◻";
				case 170:
					return "✦";
				case 171:
					return "★";
				case 172:
					return "✶";
				case 173:
					return "✴";
				case 174:
					return "✹";
				case 175:
					return "✵";
				case 177:
					return "⌖";
				case 178:
					return "⟡";
				case 179:
					return "⌑";
				case 181:
					return "✪";
				case 182:
					return "✰";
				case 195:
				case 197:
				case 215:
				case 219:
				case 223:
				case 231:
					return "◀";
				case 196:
				case 198:
				case 224:
					return "▶";
				case 213:
					return "⌫";
				case 214:
					return "⌦";
				case 216:
					return "➢";
				case 220:
					return "➲";
				case 232:
					return "➔";
				case 233:
				case 199:
				case 200:
				case 217:
				case 221:
				case 225:
					return "◭";
				case 234:
				case 201:
				case 202:
				case 218:
				case 222:
				case 226:
					return "⧨";
				case 239:
					return "⇦";
				case 240:
					return "⇨";
				case 241:
					return "◭";
				case 242:
					return "⧨";
				case 243:
					return "⬄";
				case 244:
					return "⇳";
				case 245:
				case 227:
				case 235:
					return "↖";
				case 246:
				case 228:
				case 236:
					return "↗";
				case 247:
				case 229:
				case 237:
					return "↙";
				case 248:
				case 230:
				case 238:
					return "↘";
				case 249:
					return "▭";
				case 250:
					return "▫";
				case 251:
					return "✗";
				case 252:
					return "✓";
				case 253:
					return "☒";
				case 254:
					return "☑";
				default:
					return xArrow;
			}
		}
	}

}
