#include "stdafx.h"
#include "LevelTest.h"

NJS_MATERIAL matlist_00C2F4D0[] = {
	{ { 0xFFBFBFBF }, { 0xFFFFFFFF }, 6, 0, NJD_D_100 | NJD_DA_INV_SRC | NJD_SA_SRC }
};

Sint16 poly_00C2F4E4[] = {
	0, 1, 3, 2
};

NJS_MESHSET meshlist_00C2F4EC[] = {
	{ NJD_MESHSET_4 | 0, 1, poly_00C2F4E4, NULL, NULL, NULL, NULL }
};

NJS_VECTOR vertex_00C2F508[] = {
	{ -2849, 0, -1703 },
	{ -2878, 0, 3944 },
	{ 5000, 0, 3934 },
	{ 5000, 0, -1718 }
};

NJS_VECTOR normal_00C2F538[] = {
	{ 0, 1, 0 },
	{ 0, 1, 0 },
	{ 0, 1, 0 },
	{ 0, 1, 0 }
};

NJS_MODEL attach_00C2F568 = { vertex_00C2F508, normal_00C2F538, LengthOfArray<Sint32>(vertex_00C2F508), meshlist_00C2F4EC, matlist_00C2F4D0, LengthOfArray<Uint16>(meshlist_00C2F4EC), LengthOfArray<Uint16>(matlist_00C2F4D0), { 2461, 0, 1113 }, 6043.135f };

NJS_OBJECT object_00C2F594 = { NJD_EVAL_UNIT_ANG | NJD_EVAL_UNIT_SCL | NJD_EVAL_BREAK, &attach_00C2F568, 0, -27.51823f, 0, 0, 0, 0, 1, 1, 1, NULL, NULL };

NJS_MATERIAL matlist_00C2F7B8[] = {
	{ { 0xFFBFBFBF }, { 0xFFFFFFFF }, 6, 0, NJD_D_100 | NJD_FILTER_BILINEAR | NJD_DA_INV_SRC | NJD_SA_SRC }
};

Sint16 poly_00C2F7CC[] = {
	0, 1, 3, 2
};

NJS_MESHSET_SADX meshlist_00C2F7D4[] = {
	{ NJD_MESHSET_4 | 0, 1, poly_00C2F7CC, NULL, NULL, NULL, NULL, NULL }
};

NJS_VECTOR vertex_00C2F7F0[] = {
	{ 3132.645f, -269.1f, -731.2706f },
	{ 3217.608f, -269.1f, 3535.542f },
	{ 7397.807f, -269.1f, 3688.535f },
	{ 7397.807f, -269.1f, -816.2668f }
};

NJS_VECTOR normal_00C2F820[] = {
	{ 0, 1, 0 },
	{ 0, 1, 0 },
	{ 0, 1, 0 },
	{ 0, 1, 0 }
};

NJS_MODEL_SADX attach_00C2F850 = { vertex_00C2F7F0, normal_00C2F820, LengthOfArray<Sint32>(vertex_00C2F7F0), meshlist_00C2F7D4, matlist_00C2F7B8, LengthOfArray<Uint16>(meshlist_00C2F7D4), LengthOfArray<Uint16>(matlist_00C2F7B8), { 5265.226f, -269.1f, 1436.134f }, 3101.808f, NULL };

NJS_OBJECT object_00C2F87C = { NJD_EVAL_UNIT_POS | NJD_EVAL_UNIT_ANG | NJD_EVAL_UNIT_SCL | NJD_EVAL_BREAK, &attach_00C2F850, 0, 0, 0, 0, 0, 0, 1, 1, 1, NULL, NULL };

NJS_MATERIAL matlist_00C2F5C8[] = {
	{ { 0xFFBFBFBF }, { 0xFFFFFFFF }, 6, 0, NJD_D_100 | NJD_DA_INV_SRC | NJD_SA_SRC }
};

Sint16 poly_00C2F5DC[] = {
	0, 1, 3, 2
};

NJS_MESHSET meshlist_00C2F5E4[] = {
	{ NJD_MESHSET_4 | 0, 1, poly_00C2F5DC, NULL, NULL, NULL, NULL }
};

NJS_VECTOR vertex_00C2F600[] = {
	{ -1912.818f, -23.09397f, -6691.638f },
	{ -1738.792f, -23.09397f, -1173.0743f },
	{ 8303.491f, -23.09397f, -1377.8447f },
	{ 8221.597f, -23.09397f, -6435.675f }
};

NJS_VECTOR normal_00C2F630[] = {
	{ 0, 1, 0 },
	{ 0, 1, 0 },
	{ 0, 1, 0 },
	{ 0, 1, 0 }
};

NJS_MODEL attach_00C2F660 = { vertex_00C2F600, normal_00C2F630, LengthOfArray<Sint32>(vertex_00C2F600), meshlist_00C2F5E4, matlist_00C2F5C8, LengthOfArray<Uint16>(meshlist_00C2F5E4), LengthOfArray<Uint16>(matlist_00C2F5C8), { 3195.337f, -23.09397f, -3932.356f }, 5805.762f };

NJS_OBJECT object_00C2F68C = { NJD_EVAL_UNIT_POS | NJD_EVAL_UNIT_ANG | NJD_EVAL_UNIT_SCL | NJD_EVAL_BREAK, &attach_00C2F660, 0, 0, 0, 0, 0, 0, 1, 1, 1, NULL, NULL };

NJS_MATERIAL matlist_00C2F6C0[] = {
	{ { 0xFFBFBFBF }, { 0xFFFFFFFF }, 6, 0, NJD_D_100 | NJD_FILTER_BILINEAR | NJD_DA_INV_SRC | NJD_SA_SRC }
};

Sint16 poly_00C2F6D4[] = {
	0, 1, 3, 2
};

NJS_MESHSET meshlist_00C2F6DC[] = {
	{ NJD_MESHSET_4 | 0, 1, poly_00C2F6D4, NULL, NULL, NULL, NULL }
};

NJS_VECTOR vertex_00C2F6F8[] = {
	{ -775.3598f, 569.1978f, -4428.054f },
	{ -805.4694f, 569.1978f, -3202.387f },
	{ 263.4196f, 569.1978f, -3208.41f },
	{ 754.2053f, 569.1978f, -4262.424f }
};

NJS_VECTOR normal_00C2F728[] = {
	{ 0, 1, 0 },
	{ 0, 1, 0 },
	{ 0, 1, 0 },
	{ 0, 1, 0 }
};

NJS_MODEL attach_00C2F758 = { vertex_00C2F6F8, normal_00C2F728, LengthOfArray<Sint32>(vertex_00C2F6F8), meshlist_00C2F6DC, matlist_00C2F6C0, LengthOfArray<Uint16>(meshlist_00C2F6DC), LengthOfArray<Uint16>(matlist_00C2F6C0), { -25.63202f, 569.1978f, -3815.221f }, 991.8221f };

NJS_OBJECT object_00C2F784 = { NJD_EVAL_UNIT_POS | NJD_EVAL_UNIT_ANG | NJD_EVAL_UNIT_SCL | NJD_EVAL_BREAK, &attach_00C2F758, 0, 0, -2000, 0, 0, 0, 1, 1, 1, NULL, NULL };

DeathZone EmeraldCoast1DeathZones[] = {
	{ 0x203FFFF, &object_00C2F594 },
	{ 0x3FFFF, &object_00C2F87C },
	{ 0x203FFFF, &object_00C2F68C },
	{ 0x3FFFF, &object_00C2F784 },
	{ 0 }
};
