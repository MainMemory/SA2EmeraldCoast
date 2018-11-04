#pragma once
#include "SA2ModLoader.h"

extern DeathZone EmeraldCoast1DeathZones[];
extern LoopHead *PathList_EmeraldCoast0[];
extern NJS_TEXLIST BEACH_SEA_TEXLIST;
extern NJS_TEXLIST BG_BEACH_TEXLIST;
extern NJS_OBJECT *stru_103B270;
extern NJS_OBJECT *stru_103B37C;

void __cdecl Obj_EC1Water_Display(ObjectMaster *obj);
void __cdecl Background_Display(ObjectMaster *obj);
void __cdecl Obj_EmeraldCoast_InitOceanData();

struct RenderInfoThing
{
	char gap0[8];
	int texparplus4;
	int Thing;
	int unknown2;
	int texparbuf;
	int unknown3;
	int unknown4;
	NJS_TEXLIST *CurrentTexlist;
	int unknown;
	int CurrentTexid;
};

DataPointer(RenderInfoThing *, RenderInfo, 0x2670544);

FunctionPointer(void, sub_42E730, (NJS_OBJECT *), 0x42E730);

static inline void njSetTextureNum(int texid)
{
	NJS_TEXMEMLIST *tex = (NJS_TEXMEMLIST*)RenderInfo->CurrentTexlist->textures[texid].texaddr;
	RenderInfo->texparplus4 = tex->texparambuffer + 4;
	RenderInfo->CurrentTexid = texid;
	RenderInfo->texparbuf = *(int *)tex->texparambuffer;
}

