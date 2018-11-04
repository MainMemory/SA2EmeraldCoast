#include "stdafx.h"
#include "LevelTest.h"
#include <cmath>

struct FVFStruct_H_B
{
	NJS_VECTOR position;
	float u;
	float v;
	int diffuse;
};

struct OceanGarbage
{
	FVFStruct_H_B points[4];
};

struct OceanData
{
	NJS_VECTOR Position;
	char TextureIndex;
	char OtherIndexIdk;
	char PlaneCount;
	char PrimitiveCount;
	NJS_POINT2 Offset;
	char VBuffIndex;
	char field_19;
	char field_1A;
	char field_1B;
};

// void __usercall(int a1@<eax>, int a2)
static const void *const njColorBlendingModePtr = (void*)0x426420;
static inline void njColorBlendingMode(int a1, int a2)
{
	__asm
	{
		push[a2]
		mov eax, [a1]
		call njColorBlendingModePtr
		add esp, 4
	}
}

OceanData OceanDataArray[4];
OceanGarbage OceanGarbageArray[140];

// void __usercall(FVFStruct_H_B *a1@<eax>, signed int vertexCount@<ecx>)
static const void *const DrawFVF_HPtr = (void*)0x781370;
static inline void DrawFVF_H(FVFStruct_H_B *a1, signed int vertexCount)
{
	__asm
	{
		mov ecx, [vertexCount]
		mov eax, [a1]
		call DrawFVF_HPtr
	}
}

short word_7EC250[] = { 0x400, 0x401, 0x402, 0x403, 0x404, 0x405, 0x406, 0x407, 0x308, 0x209, 0x20A, 0x20B, 0x20C, 0x20D, 0x20E, 0xFF };

// void __usercall(float *a1@<ecx>, int a2)
static const void *const sub_41FDE0Ptr = (void*)0x41FDE0;
static inline void sub_41FDE0(float *a1, int a2)
{
	__asm
	{
		push[a2]
		mov ecx, [a1]
		call sub_41FDE0Ptr
		add esp, 4
	}
}

// void __usercall(int a1@<eax>)
static const void *const sub_41B1F0Ptr = (void*)0x41B1F0;
static inline void sub_41B1F0(int a1)
{
	__asm
	{
		mov eax, [a1]
		call sub_41B1F0Ptr
	}
}

VoidFunc(sub_4293B0, 0x4293B0);
VoidFunc(sub_4292E0, 0x4292E0);
FastcallFunctionPointer(double, njSin, (int a1), 0x42AAB0);
void __cdecl EmeraldCoast_OceanDraw(OceanData *o)
{
	char v2; // al
	char v3; // cl
	double f; // st7
	double z; // ST1C_4
	double x; // ST14_4
	int v7; // edi
	int v8; // edi
	double _f; // [esp+18h] [ebp+4h]
	double xb; // [esp+18h] [ebp+4h]

	njPushMatrix(0);
	if (GameState != GameStates_Pause)
	{
		v2 = o->OtherIndexIdk--;
		if (!v2)
		{
			v3 = o->TextureIndex + 1;
			o->TextureIndex = v3;
			if ((char)word_7EC250[v3] == -1)
			{
				o->TextureIndex = 0;
			}
			o->OtherIndexIdk = word_7EC250[o->TextureIndex] >> 8;
		}
	}
	f = njSin(FrameCountIngame << 6) * o->Offset.y;
	_f = f;
	z = f + o->Position.z;
	x = _f + o->Position.x;
	njTranslate(_nj_current_matrix_ptr_, (float)x, o->Position.y, (float)z);
	njPushMatrix(0);
	xb = _f * 0.5;
	njTranslate(_nj_current_matrix_ptr_, (float)xb, -1.0, (float)xb);
	njSetTextureNum((char)word_7EC250[o->TextureIndex]);
	v7 = 0;
	if (o->PlaneCount)
	{
		do
		{
			sub_41FDE0(_nj_current_matrix_ptr_, 0);
			sub_41B1F0(1);
			sub_4293B0();
			sub_4292E0();
			DrawFVF_H(
				OceanGarbageArray[0x23 * o->VBuffIndex].points,
				4 * o->PrimitiveCount);
			njTranslate(_nj_current_matrix_ptr_, o->Offset.x, 0.0, 0.0);
			++v7;
		} while (v7 < o->PlaneCount);
	}
	njPopMatrix(1u);
	njPushMatrix(0);
	njSetTextureNum((o->VBuffIndex & 1) + 15);
	v8 = 0;
	if (o->PlaneCount)
	{
		do
		{
			sub_41FDE0(_nj_current_matrix_ptr_, 0);
			sub_41B1F0(1);
			sub_4293B0();
			sub_4292E0();
			DrawFVF_H(
				OceanGarbageArray[0x23 * o->VBuffIndex].points,
				4 * o->PrimitiveCount);
			njTranslate(_nj_current_matrix_ptr_, o->Offset.x, 0.0, 0.0);
			++v8;
		} while (v8 < o->PlaneCount);
	}
	njPopMatrix(1u);
	njPopMatrix(1u);
}

void __cdecl Obj_EC1Water_Display(ObjectMaster *obj)
{
	//Direct3D_SetZFunc(3u);
	//Direct3D_EnableZWrite(0);
	njColorBlendingMode(NJD_SOURCE_COLOR, NJD_COLOR_BLENDING_SRCALPHA);
	njColorBlendingMode(NJD_DESTINATION_COLOR, NJD_COLOR_BLENDING_ONE);
	RenderInfo->CurrentTexlist = &BEACH_SEA_TEXLIST;
	//SetOceanAlphaModeAndFVF(1);
	for (int i = 0; i < 4; i++)
		EmeraldCoast_OceanDraw(&OceanDataArray[i]);
	//Direct3D_ResetZFunc();
	//Direct3D_EnableZWrite(1u);
	RenderInfo->Thing = RenderInfo->Thing & 0x3FE7FFF | 0x94018000;
}

DataPointer(char, byte_174AFFD, 0x174AFFD);
DataPointer(int, playerNum, 0x1DD92A0);
DataArray(NJS_VECTOR *, CameraPosPtrs, 0x1DD92B0, 2);
FunctionPointer(void, DrawLensFlare, (NJS_VECTOR *), 0x6C7600);
void __cdecl Background_Display(ObjectMaster *obj)
{
	EntityData1 *v1; // ebx
	float v3; // ST0C_4
	NJS_VECTOR a1; // [esp+10h] [ebp-Ch]

	bool b;
	if (byte_174AFFD == 1)
	{
		b = playerNum == 0;
	}
	else if (byte_174AFFD == 2)
	{
		b = playerNum == 1;
	}
	else
	{
		b = byte_174AFFD == 3;
	}
	if (!b)
	{
		NJS_VECTOR *v2 = CameraPosPtrs[playerNum];
		v1 = MainCharObj1[playerNum];
		if (v1->Position.z >= -1000)
		{
			njPushMatrix(0);
			njTranslate(_nj_current_matrix_ptr_, v2->x, 0.0, v2->z);
			RenderInfo->CurrentTexlist = &BG_BEACH_TEXLIST;
			njScale(1.45f, 1.23f, 1.11f);
			sub_42E730(stru_103B270);
			sub_42E730(stru_103B37C);
			njPopMatrix(1u);
			a1.x = 10000.0;
			a1.y = 4000.0;
			a1.z = -590.0;
			DrawLensFlare(&a1);
			a1.x = -1500.0;
			a1.y = 3000.0;
			a1.z = 4000.0;
			DrawLensFlare(&a1);
		}
		else
		{
			v3 = (v1->Position.x - 6235) * (v1->Position.x - 6235)
				+ v1->Position.y * v1->Position.y
				+ (v1->Position.z - -2275) * (v1->Position.z - -2275);
			if (sqrt(v3) < 1000.0)
			{
				a1.x = 17000.0;
				a1.y = 3500.0;
				a1.z = -2000.0;
				DrawLensFlare(&a1);
			}
			njPushMatrix(0);
			njTranslate(_nj_current_matrix_ptr_, v2->x, 0.0, v2->z);
			RenderInfo->CurrentTexlist = &BG_BEACH_TEXLIST;
			njScale(1.3f, 1.3f, 1.3f);
			sub_42E730(stru_103B270);
			sub_42E730(stru_103B37C);
			njPopMatrix(1u);
		}
	}
}

void __cdecl InitOceanData(int n)
{
	double v2; // st7
	int i; // ebp
	OceanGarbage *v5; // eax
	double v6; // [esp+0h] [ebp-4h]

	v2 = 0.0;
	if (OceanDataArray[n].PrimitiveCount)
	{
		i = (unsigned __int8)OceanDataArray[n].PrimitiveCount;
		v6 = OceanDataArray[n].Offset.x;
		v5 = &OceanGarbageArray[35 * n];            // fml
		do
		{
			v5->points[1].position.z = (float)v2;
			v5->points[0].position.z = (float)v2;
			v5->points[2].position.x = 0.0;
			v2 = v2 + v6;
			v5->points[0].position.x = 0.0;
			v5->points[3].position.x = (float)v6;
			v5->points[1].position.x = (float)v6;
			v5->points[3].position.z = (float)v2;
			v5->points[3].position.y = 0.0;
			v5->points[2].position.z = (float)v2;
			v5->points[2].position.y = 0.0;
			v5->points[1].position.y = 0.0;
			v5->points[0].position.y = 0.0;
			v5->points[2].v = 0.0;
			v5->points[0].v = 0.0;
			v5->points[1].u = 0.0;
			v5->points[0].u = 0.0;
			v5->points[3].v = 1.0;
			v5->points[1].v = 1.0;
			v5->points[3].u = 1.0;
			v5->points[2].u = 1.0;
			v5->points[3].diffuse = 0xFFFFFFFF;
			v5->points[2].diffuse = 0xFFFFFFFF;
			v5->points[1].diffuse = 0xFFFFFFFF;
			v5->points[0].diffuse = 0xFFFFFFFF;
			++v5;
			--i;
		} while (i);
	}
	OceanDataArray[n].VBuffIndex = n;
}

void __cdecl Obj_EmeraldCoast_InitOceanData()
{
	OceanDataArray[0].Position.x = -3000.0;
	OceanDataArray[0].Position.z = -1000.0;
	OceanDataArray[0].Position.y = -0.5;
	OceanDataArray[0].PlaneCount = 90;
	OceanDataArray[0].PrimitiveCount = 35;
	OceanDataArray[0].Offset.x = 128.0;
	OceanDataArray[0].Offset.y = 40.0;
	InitOceanData(0);
	OceanDataArray[0].TextureIndex = 0;
	OceanDataArray[0].OtherIndexIdk = 4;
	OceanDataArray[1].Position.x = 5845.0;
	OceanDataArray[1].Position.z = 2050.0;
	OceanDataArray[1].Position.y = -108.5;
	OceanDataArray[1].PlaneCount = 10;
	OceanDataArray[1].PrimitiveCount = 10;
	OceanDataArray[1].Offset.y = 5.0;
	OceanDataArray[1].Offset.x = 64.0;
	InitOceanData(1);
	OceanDataArray[1].TextureIndex = 0;
	OceanDataArray[1].OtherIndexIdk = 4;
	OceanDataArray[2].Position.x = 700.0;
	OceanDataArray[2].Position.z = -6850.0;
	OceanDataArray[2].Position.y = -1.5;
	OceanDataArray[2].PlaneCount = 0x3C;
	OceanDataArray[2].PrimitiveCount = 35;
	OceanDataArray[2].Offset.x = 128.0;
	OceanDataArray[2].Offset.y = 40.0;
	InitOceanData(2);
	OceanDataArray[2].TextureIndex = 0;
	OceanDataArray[2].OtherIndexIdk = 4;
	OceanDataArray[3].Position.x = -630.0;
	OceanDataArray[3].Position.z = -4160.0;
	OceanDataArray[3].Position.y = 540.0;
	OceanDataArray[3].PlaneCount = 27;
	OceanDataArray[3].PrimitiveCount = 23;
	OceanDataArray[3].Offset.y = 10.0;
	OceanDataArray[3].Offset.x = 64.0;
	InitOceanData(3);
	OceanDataArray[3].TextureIndex = 0;
	OceanDataArray[3].OtherIndexIdk = 4;
}