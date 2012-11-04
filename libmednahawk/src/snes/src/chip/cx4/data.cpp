#ifdef CX4_CPP

const uint8 Cx4::immediate_data[48] = {
  0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0x00, 0xff, 0x00, 0x00, 0x00, 0xff,
  0xff, 0xff, 0x00, 0x00, 0xff, 0xff, 0x00, 0x00, 0x80, 0xff, 0xff, 0x7f,
  0x00, 0x80, 0x00, 0xff, 0x7f, 0x00, 0xff, 0x7f, 0xff, 0x7f, 0xff, 0xff,
  0x00, 0x00, 0x01, 0xff, 0xff, 0xfe, 0x00, 0x01, 0x00, 0xff, 0xfe, 0x00
};

const uint16 Cx4::wave_data[40] = {
  0x0000, 0x0002, 0x0004, 0x0006, 0x0008, 0x000a, 0x000c, 0x000e,
  0x0200, 0x0202, 0x0204, 0x0206, 0x0208, 0x020a, 0x020c, 0x020e,
  0x0400, 0x0402, 0x0404, 0x0406, 0x0408, 0x040a, 0x040c, 0x040e,
  0x0600, 0x0602, 0x0604, 0x0606, 0x0608, 0x060a, 0x060c, 0x060e,
  0x0800, 0x0802, 0x0804, 0x0806, 0x0808, 0x080a, 0x080c, 0x080e
};

const uint32 Cx4::sin_table[256] = {
  0x000000, 0x000324, 0x000648, 0x00096c, 0x000c8f, 0x000fb2, 0x0012d5, 0x0015f6,
  0x001917, 0x001c37, 0x001f56, 0x002273, 0x002590, 0x0028aa, 0x002bc4, 0x002edb,
  0x0031f1, 0x003505, 0x003817, 0x003b26, 0x003e33, 0x00413e, 0x004447, 0x00474d,
  0x004a50, 0x004d50, 0x00504d, 0x005347, 0x00563e, 0x005931, 0x005c22, 0x005f0e,
  0x0061f7, 0x0064dc, 0x0067bd, 0x006a9b, 0x006d74, 0x007049, 0x007319, 0x0075e5,
  0x0078ad, 0x007b70, 0x007e2e, 0x0080e7, 0x00839c, 0x00864b, 0x0088f5, 0x008b9a,
  0x008e39, 0x0090d3, 0x009368, 0x0095f6, 0x00987f, 0x009b02, 0x009d7f, 0x009ff6,
  0x00a267, 0x00a4d2, 0x00a736, 0x00a994, 0x00abeb, 0x00ae3b, 0x00b085, 0x00b2c8,
  0x00b504, 0x00b73a, 0x00b968, 0x00bb8f, 0x00bdae, 0x00bfc7, 0x00c1d8, 0x00c3e2,
  0x00c5e4, 0x00c7de, 0x00c9d1, 0x00cbbb, 0x00cd9f, 0x00cf7a, 0x00d14d, 0x00d318,
  0x00d4db, 0x00d695, 0x00d848, 0x00d9f2, 0x00db94, 0x00dd2d, 0x00debe, 0x00e046,
  0x00e1c5, 0x00e33c, 0x00e4aa, 0x00e60f, 0x00e76b, 0x00e8bf, 0x00ea09, 0x00eb4b,
  0x00ec83, 0x00edb2, 0x00eed8, 0x00eff5, 0x00f109, 0x00f213, 0x00f314, 0x00f40b,
  0x00f4fa, 0x00f5de, 0x00f6ba, 0x00f78b, 0x00f853, 0x00f912, 0x00f9c7, 0x00fa73,
  0x00fb14, 0x00fbac, 0x00fc3b, 0x00fcbf, 0x00fd3a, 0x00fdab, 0x00fe13, 0x00fe70,
  0x00fec4, 0x00ff0e, 0x00ff4e, 0x00ff84, 0x00ffb1, 0x00ffd3, 0x00ffec, 0x00fffb,
  0x000000, 0xfffcdb, 0xfff9b7, 0xfff693, 0xfff370, 0xfff04d, 0xffed2a, 0xffea09,
  0xffe6e8, 0xffe3c8, 0xffe0a9, 0xffdd8c, 0xffda6f, 0xffd755, 0xffd43b, 0xffd124,
  0xffce0e, 0xffcafa, 0xffc7e8, 0xffc4d9, 0xffc1cc, 0xffbec1, 0xffbbb8, 0xffb8b2,
  0xffb5af, 0xffb2af, 0xffafb2, 0xffacb8, 0xffa9c1, 0xffa6ce, 0xffa3dd, 0xffa0f1,
  0xff9e08, 0xff9b23, 0xff9842, 0xff9564, 0xff928b, 0xff8fb6, 0xff8ce6, 0xff8a1a,
  0xff8752, 0xff848f, 0xff81d1, 0xff7f18, 0xff7c63, 0xff79b4, 0xff770a, 0xff7465,
  0xff71c6, 0xff6f2c, 0xff6c97, 0xff6a09, 0xff6780, 0xff64fd, 0xff6280, 0xff6009,
  0xff5d98, 0xff5b2d, 0xff58c9, 0xff566b, 0xff5414, 0xff51c4, 0xff4f7a, 0xff4d37,
  0xff4afb, 0xff48c5, 0xff4697, 0xff4470, 0xff4251, 0xff4038, 0xff3e27, 0xff3c1e,
  0xff3a1b, 0xff3821, 0xff362e, 0xff3444, 0xff3260, 0xff3085, 0xff2eb2, 0xff2ce7,
  0xff2b24, 0xff296a, 0xff27b7, 0xff260d, 0xff246b, 0xff22d2, 0xff2141, 0xff1fb9,
  0xff1e3a, 0xff1cc3, 0xff1b55, 0xff19f0, 0xff1894, 0xff1740, 0xff15f6, 0xff14b4,
  0xff137c, 0xff124d, 0xff1127, 0xff100a, 0xff0ef6, 0xff0dec, 0xff0ceb, 0xff0bf4,
  0xff0b05, 0xff0a21, 0xff0945, 0xff0874, 0xff07ac, 0xff06ed, 0xff0638, 0xff058d,
  0xff04eb, 0xff0453, 0xff03c4, 0xff0340, 0xff02c5, 0xff0254, 0xff01ec, 0xff018f,
  0xff013b, 0xff00f1, 0xff00b1, 0xff007b, 0xff004e, 0xff002c, 0xff0013, 0xff0004
};

const int16 Cx4::SinTable[512] = {
       0,    402,    804,   1206,   1607,   2009,   2410,   2811,
    3211,   3611,   4011,   4409,   4808,   5205,   5602,   5997,
    6392,   6786,   7179,   7571,   7961,   8351,   8739,   9126,
    9512,   9896,  10278,  10659,  11039,  11416,  11793,  12167,
   12539,  12910,  13278,  13645,  14010,  14372,  14732,  15090,
   15446,  15800,  16151,  16499,  16846,  17189,  17530,  17869,
   18204,  18537,  18868,  19195,  19519,  19841,  20159,  20475,
   20787,  21097,  21403,  21706,  22005,  22301,  22594,  22884,
   23170,  23453,  23732,  24007,  24279,  24547,  24812,  25073,
   25330,  25583,  25832,  26077,  26319,  26557,  26790,  27020,
   27245,  27466,  27684,  27897,  28106,  28310,  28511,  28707,
   28898,  29086,  29269,  29447,  29621,  29791,  29956,  30117,
   30273,  30425,  30572,  30714,  30852,  30985,  31114,  31237,
   31357,  31471,  31581,  31685,  31785,  31881,  31971,  32057,
   32138,  32214,  32285,  32351,  32413,  32469,  32521,  32568,
   32610,  32647,  32679,  32706,  32728,  32745,  32758,  32765,
   32767,  32765,  32758,  32745,  32728,  32706,  32679,  32647,
   32610,  32568,  32521,  32469,  32413,  32351,  32285,  32214,
   32138,  32057,  31971,  31881,  31785,  31685,  31581,  31471,
   31357,  31237,  31114,  30985,  30852,  30714,  30572,  30425,
   30273,  30117,  29956,  29791,  29621,  29447,  29269,  29086,
   28898,  28707,  28511,  28310,  28106,  27897,  27684,  27466,
   27245,  27020,  26790,  26557,  26319,  26077,  25832,  25583,
   25330,  25073,  24812,  24547,  24279,  24007,  23732,  23453,
   23170,  22884,  22594,  22301,  22005,  21706,  21403,  21097,
   20787,  20475,  20159,  19841,  19519,  19195,  18868,  18537,
   18204,  17869,  17530,  17189,  16846,  16499,  16151,  15800,
   15446,  15090,  14732,  14372,  14010,  13645,  13278,  12910,
   12539,  12167,  11793,  11416,  11039,  10659,  10278,   9896,
    9512,   9126,   8739,   8351,   7961,   7571,   7179,   6786,
    6392,   5997,   5602,   5205,   4808,   4409,   4011,   3611,
    3211,   2811,   2410,   2009,   1607,   1206,    804,    402,
       0,   -402,   -804,  -1206,  -1607,  -2009,  -2410,  -2811,
   -3211,  -3611,  -4011,  -4409,  -4808,  -5205,  -5602,  -5997,
   -6392,  -6786,  -7179,  -7571,  -7961,  -8351,  -8739,  -9126,
   -9512,  -9896, -10278, -10659, -11039, -11416, -11793, -12167,
  -12539, -12910, -13278, -13645, -14010, -14372, -14732, -15090,
  -15446, -15800, -16151, -16499, -16846, -17189, -17530, -17869,
  -18204, -18537, -18868, -19195, -19519, -19841, -20159, -20475,
  -20787, -21097, -21403, -21706, -22005, -22301, -22594, -22884,
  -23170, -23453, -23732, -24007, -24279, -24547, -24812, -25073,
  -25330, -25583, -25832, -26077, -26319, -26557, -26790, -27020,
  -27245, -27466, -27684, -27897, -28106, -28310, -28511, -28707,
  -28898, -29086, -29269, -29447, -29621, -29791, -29956, -30117,
  -30273, -30425, -30572, -30714, -30852, -30985, -31114, -31237,
  -31357, -31471, -31581, -31685, -31785, -31881, -31971, -32057,
  -32138, -32214, -32285, -32351, -32413, -32469, -32521, -32568,
  -32610, -32647, -32679, -32706, -32728, -32745, -32758, -32765,
  -32767, -32765, -32758, -32745, -32728, -32706, -32679, -32647,
  -32610, -32568, -32521, -32469, -32413, -32351, -32285, -32214,
  -32138, -32057, -31971, -31881, -31785, -31685, -31581, -31471,
  -31357, -31237, -31114, -30985, -30852, -30714, -30572, -30425,
  -30273, -30117, -29956, -29791, -29621, -29447, -29269, -29086,
  -28898, -28707, -28511, -28310, -28106, -27897, -27684, -27466,
  -27245, -27020, -26790, -26557, -26319, -26077, -25832, -25583,
  -25330, -25073, -24812, -24547, -24279, -24007, -23732, -23453,
  -23170, -22884, -22594, -22301, -22005, -21706, -21403, -21097,
  -20787, -20475, -20159, -19841, -19519, -19195, -18868, -18537,
  -18204, -17869, -17530, -17189, -16846, -16499, -16151, -15800,
  -15446, -15090, -14732, -14372, -14010, -13645, -13278, -12910,
  -12539, -12167, -11793, -11416, -11039, -10659, -10278,  -9896,
   -9512,  -9126,  -8739,  -8351,  -7961,  -7571,  -7179,  -6786,
   -6392,  -5997,  -5602,  -5205,  -4808,  -4409,  -4011,  -3611,
   -3211,  -2811,  -2410,  -2009,  -1607,  -1206,   -804,   -402
};

const int16 Cx4::CosTable[512] = {
   32767,  32765,  32758,  32745,  32728,  32706,  32679,  32647,
   32610,  32568,  32521,  32469,  32413,  32351,  32285,  32214,
   32138,  32057,  31971,  31881,  31785,  31685,  31581,  31471,
   31357,  31237,  31114,  30985,  30852,  30714,  30572,  30425,
   30273,  30117,  29956,  29791,  29621,  29447,  29269,  29086,
   28898,  28707,  28511,  28310,  28106,  27897,  27684,  27466,
   27245,  27020,  26790,  26557,  26319,  26077,  25832,  25583,
   25330,  25073,  24812,  24547,  24279,  24007,  23732,  23453,
   23170,  22884,  22594,  22301,  22005,  21706,  21403,  21097,
   20787,  20475,  20159,  19841,  19519,  19195,  18868,  18537,
   18204,  17869,  17530,  17189,  16846,  16499,  16151,  15800,
   15446,  15090,  14732,  14372,  14010,  13645,  13278,  12910,
   12539,  12167,  11793,  11416,  11039,  10659,  10278,   9896,
    9512,   9126,   8739,   8351,   7961,   7571,   7179,   6786,
    6392,   5997,   5602,   5205,   4808,   4409,   4011,   3611,
    3211,   2811,   2410,   2009,   1607,   1206,    804,    402,
       0,   -402,   -804,  -1206,  -1607,  -2009,  -2410,  -2811,
   -3211,  -3611,  -4011,  -4409,  -4808,  -5205,  -5602,  -5997,
   -6392,  -6786,  -7179,  -7571,  -7961,  -8351,  -8739,  -9126,
   -9512,  -9896, -10278, -10659, -11039, -11416, -11793, -12167,
  -12539, -12910, -13278, -13645, -14010, -14372, -14732, -15090,
  -15446, -15800, -16151, -16499, -16846, -17189, -17530, -17869,
  -18204, -18537, -18868, -19195, -19519, -19841, -20159, -20475,
  -20787, -21097, -21403, -21706, -22005, -22301, -22594, -22884,
  -23170, -23453, -23732, -24007, -24279, -24547, -24812, -25073,
  -25330, -25583, -25832, -26077, -26319, -26557, -26790, -27020,
  -27245, -27466, -27684, -27897, -28106, -28310, -28511, -28707,
  -28898, -29086, -29269, -29447, -29621, -29791, -29956, -30117,
  -30273, -30425, -30572, -30714, -30852, -30985, -31114, -31237,
  -31357, -31471, -31581, -31685, -31785, -31881, -31971, -32057,
  -32138, -32214, -32285, -32351, -32413, -32469, -32521, -32568,
  -32610, -32647, -32679, -32706, -32728, -32745, -32758, -32765,
  -32767, -32765, -32758, -32745, -32728, -32706, -32679, -32647,
  -32610, -32568, -32521, -32469, -32413, -32351, -32285, -32214,
  -32138, -32057, -31971, -31881, -31785, -31685, -31581, -31471,
  -31357, -31237, -31114, -30985, -30852, -30714, -30572, -30425,
  -30273, -30117, -29956, -29791, -29621, -29447, -29269, -29086,
  -28898, -28707, -28511, -28310, -28106, -27897, -27684, -27466,
  -27245, -27020, -26790, -26557, -26319, -26077, -25832, -25583,
  -25330, -25073, -24812, -24547, -24279, -24007, -23732, -23453,
  -23170, -22884, -22594, -22301, -22005, -21706, -21403, -21097,
  -20787, -20475, -20159, -19841, -19519, -19195, -18868, -18537,
  -18204, -17869, -17530, -17189, -16846, -16499, -16151, -15800,
  -15446, -15090, -14732, -14372, -14010, -13645, -13278, -12910,
  -12539, -12167, -11793, -11416, -11039, -10659, -10278,  -9896,
   -9512,  -9126,  -8739,  -8351,  -7961,  -7571,  -7179,  -6786,
   -6392,  -5997,  -5602,  -5205,  -4808,  -4409,  -4011,  -3611,
   -3211,  -2811,  -2410,  -2009,  -1607,  -1206,   -804,   -402,
       0,    402,    804,   1206,   1607,   2009,   2410,   2811,
    3211,   3611,   4011,   4409,   4808,   5205,   5602,   5997,
    6392,   6786,   7179,   7571,   7961,   8351,   8739,   9126,
    9512,   9896,  10278,  10659,  11039,  11416,  11793,  12167,
   12539,  12910,  13278,  13645,  14010,  14372,  14732,  15090,
   15446,  15800,  16151,  16499,  16846,  17189,  17530,  17869,
   18204,  18537,  18868,  19195,  19519,  19841,  20159,  20475,
   20787,  21097,  21403,  21706,  22005,  22301,  22594,  22884,
   23170,  23453,  23732,  24007,  24279,  24547,  24812,  25073,
   25330,  25583,  25832,  26077,  26319,  26557,  26790,  27020,
   27245,  27466,  27684,  27897,  28106,  28310,  28511,  28707,
   28898,  29086,  29269,  29447,  29621,  29791,  29956,  30117,
   30273,  30425,  30572,  30714,  30852,  30985,  31114,  31237,
   31357,  31471,  31581,  31685,  31785,  31881,  31971,  32057,
   32138,  32214,  32285,  32351,  32413,  32469,  32521,  32568,
   32610,  32647,  32679,  32706,  32728,  32745,  32758,  32765
};

#endif
