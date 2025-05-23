# VFXEditor [![Download count](https://img.shields.io/endpoint?url=https%3A%2F%2Fvz32sgcoal.execute-api.us-east-1.amazonaws.com%2FVFXEditor)](https://github.com/0ceal0t/Dalamud-VFXEditor)

**[Wiki](https://xiv.dev/game-data/visual-effects)** | **[Guides](https://github.com/0ceal0t/Dalamud-VFXEditor/wiki)** | Icon by [PAPACHIN](https://www.xivmodarchive.com/user/192152)

_VFX, animation, sound, and physics editing plugin for Dalamud_

![](https://github.com/0ceal0t/Dalamud-VFXEditor/assets/18051158/83273164-d216-4758-9249-a2f38c03d6c2)

> Just want to hide certain VFXs? Use [EasyEyes](https://github.com/0ceal0t/EasyEyes) instead

## 7.2 TODO
- Double-check offsets
- Double-check sigs
- Fix STM files
- Better curve colors

### Supported File Types

| Extension | Description |
| --- | --- |
| `.avfx` | VFXs, such as particles and glow effects. Has no impact on a character's motion |
| `.pap` | The animations performed by a character (such as swinging a weapon, smiling, etc.) |
| `.tmb` | Timelines for when to trigger VFXs, animations, and sound effects. Edit this if you want to replace a skill in its entirety |
| `.scd` | Sound files (background music, sound effects) |
| `.eid` | Bind points used to attach VFXs to character models |
| `.uld` | Determines the layout of UI elements |
| `.atex` | Texture files for `.avfx` |
| `.tex` | Texture files for UI elements and game models |
| `.atch` | Attachment points for weapons and other accessories (like the Machinist bag and Astrologian deck) |
| `.sklb` | Skeleton and bone definitions |
| `.skp` | Skeleton paramters, such as IK |
| `.shpk` | Bundled packages of vertex and pixel shaders |
| `.shcd` | Individual shaders |
| `.mtrl` | Materials for models |
| `.mdl` | Meshes |

## Installation
1. This plugin requires [XIV Quick Launcher](https://github.com/goatcorp/FFXIVQuickLauncher) to run
2. Install the plugin from the `/xlplugins` menu
3. Once installed, open with `/vfxedit`

### Beta Builds
Add the following custom repository in `/xlsettings > Experimental`:
```
https://raw.githubusercontent.com/0ceal0t/Dalamud-VFXEditor/main/repo.json
```

The beta and main builds cannot both be enabled at the same time, so make sure to disable one before enabling the other. Open the beta version using `/vfxbeta`

## Usage
1. Select a _"Loaded Vfx"_ (the new effect you want to use)
2. Select a _"Vfx Being Replaced"_ (the effect which is being overriden. This can be the same as the source)
3. Make any modifications you want, then press `UPDATE`

## Support
Having problems? Check [troubleshooting tips](https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Troubleshooting) or a [basic guide](https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Basic-Guide). If you have other questions, please open an [Issue](https://github.com/0ceal0t/Dalamud-VFXEditor/issues)

- [QuickLauncher Discord](https://github.com/goatcorp/FFXIVQuickLauncher#need-any-help)
- [Students of Baldesion](https://discord.gg/33jxhxH8)

## Contributing
If a VFX is behaving unexpectedly, or you are getting log messages indicating that it is not being parsed properly, please open an [Issue](https://github.com/0ceal0t/Dalamud-VFXEditor/issues).

- [.phyb research document](https://docs.google.com/document/d/1g0iSnvz9IjkGBVqXM5h3KfoyP_LOsr9LGKqiVhMZ_Us/edit)
- [.tmb/.pap research document](https://docs.google.com/document/d/1LhsTHO65pu7NcerhvoQBrYtgKyjSPggjx0JurwZVpw4/edit)
- [.scd research document](https://docs.google.com/document/d/1L9GKap9u703QJH9u1ymXCUEx4BMi1Tov4J5tvFRWp-w/edit)
- [.sklb research document](https://docs.google.com/document/d/13TBozIOwKHCMm1SMIhVUQtzaCg9bU18gDATHmXtqO1U/edit#heading=h.4fswckssvps1)

## Misc Notes
[Flatbuffer download](https://github.com/google/flatbuffers/releases)
```
.\flatc.exe --json --raw-binary .\ephb.fbs -- .\304_1.phyb
.\flatc.exe --binary .\ephb.fbs .\304_1.json
..\flatc.exe --csharp --gen-object-api --gen-onefile .\Ephb.fbs
```

## TODO
- [ ] Display multiple axes at the same time in curve editor
- [ ] Use job codes in search bar
- [ ] Add particle/emitter toggles

- [ ] Add gizmo to `.sklb` bones
- [ ] Copy/paste `.atch` entry
- [ ] `.uld` node type 10
- [ ] Fix `.scd` looping issues?
- [ ] DT stain files (https://github.com/Ottermandias/Penumbra.GameData/commit/fad2671d70df79e6b46668b5f6ac4aee35e1e078#diff-1675af0b153a065f21e2fd58777f8e294ade4a25eeee737a894fb804ab08a99c)
- [ ] DT color table (https://github.com/Ottermandias/Penumbra.GameData/commit/ffa624936a1de13a64c6fa6a5f899e41375d58b1)
- [ ] F Hrothgar icons in select dialog
- [ ] New `.pap` animation from gLTF (currently can only replace)
- [ ] Hex editor for replacing arbitrary files
- [ ] More investigation into `.pap` _Type_ and animation names
- [ ] C192
- [ ] `.uld` update component type
- [ ] More research into sound position (C063)
- [ ] Weird crackling when playing back 4-ch and 6-ch files. Maybe related to clipping/conversion?
- [ ] Make it so node names don't change when others are deleted (Particle 1, etc.)
- [ ] More accurate spline curve calculations
- [ ] Sound icon on timeline
- [ ] Investigate VFX flags, better structs
- [ ] Weapons / footsteps / etc. in live view
- [ ] Add pre and post behavior to curve editor
- [ ] Update "try on" preview

```
chara/xls/charadb/equipmentdeformerparameter/c0101.eqdp

.kdlb
.bklb
.kdb (kinedriver)

Client.System.Resource.Handle.AnimationExtensionLoadResourceHandle
Client.System.Resource.Handle.BonamikLoadResourceHandle
Client.System.Resource.Handle.BonamikResourceHandle
Client.System.Resource.Handle.ExtraSkeletonLoadResourceHandle
Client.System.Resource.Handle.EyeAnimationResourceHandle
Client.System.Resource.Handle.FacialParameterEditResourceHandle
Client.System.Resource.Handle.KineDriverLoadResourceHandle
Client.System.Resource.Handle.KineDriverResourceHandle

chara/human/c%04d/skeleton/base/b0001/bnm_c%04db0001.bnmb
chara/human/c%04d/skeleton/face/f%04d/bnm_c%04df%04d.bnmb
chara/human/c%04d/skeleton/hair/h%04d/bnm_c%04dh%04d.bnmb
chara/human/c%04d/skeleton/met/m%04d/bnm_c%04dm%04d.bnmb
chara/human/c%04d/skeleton/top/t%04d/bnm_c%04dt%04d.bnmb
chara/demihuman/d%04d/skeleton/base/b0001/bnm_d%04db0001.bnmb
chara/weapon/w%04d/skeleton/parts/p%04d/bnm_w%04dp%04d.bnmb
chara/weapon/w%04d/skeleton/base/b0001/bnm_w%04db0001.bnmb
chara/monster/m%04d/skeleton/base/b0001/bnm_m%04db0001.bnmb

https://github.com/BlasterGrim/TresGame/tree/6a0fd71b783a1fcba90c9df619447dc620817910/Plugins/BonamikRt/Source/BonamikRt/Public

common/graphics/common_shader_param.spm
common/graphics/chara_shader_param.spm
common/graphics/bg_shader_param.spm
https://imgur.com/wcMGZxY
HairSpecularShift breaks into HairSpecularPrimaryShift, HairSpecularBackScatterShift, and HairSpecularSecondaryShift; HairRoughnessOffsetRate breaks into HairBackScatterRoughnessOffsetRate, and HairSecondaryRoughnessOffsetRate


https://github.com/Irastris/ValkyrieUproject/tree/main/VALKYRIE_ELYSIUM/Source/KineDriverRt/Public
https://github.com/RussellJerome/TresGame/blob/main/Plugins/KineDriverRt/Source/KineDriverRt/Public/KineDriver_StructsAndEnums.h

chara/xls/bonamik/bonamik-monster.bklb
chara/xls/bonamik/bonamik-weapon.bklb
chara/xls/bonamik/bonamik-human-base.bklb
chara/xls/bonamik/bonamik-human-equipment.bklb
chara/xls/bonamik/bonamik-human-face.bklb
chara/xls/bonamik/bonamik-human-hair.bklb
chara/xls/bonamik/bonamik-demihuman.bklb
chara/xls/bonamik/bonamik-human-equipment.bklb
chara/xls/bonamik/bonamik-monster.bklb
chara/xls/bonamik/bonamik-weapon.bklb
chara/xls/bonamik/bonamik-demihuman.bklb
chara/xls/bonamik/bonamik-human-base.bklb
chara/xls/bonamik/bonamik-human-face.bklb
chara/xls/bonamik/bonamik-human-hair.bklb

chara/xls/kinedriver/kinedriver-human-equipment.kdlb
chara/xls/kinedriver/kinedriver-human-base.kdlb
chara/xls/kinedriver/kinedriver-human-face.kdlb
chara/xls/kinedriver/kinedriver-human-hair.kdlb
chara/xls/kinedriver/kinedriver-demihuman.kdlb
chara/xls/kinedriver/kinedriver-monster.kdlb
chara/xls/kinedriver/kinedriver-weapon.kdlb
chara/xls/kinedriver/kinedriver-demihuman.kdlb
chara/xls/kinedriver/kinedriver-human-base.kdlb
chara/xls/kinedriver/kinedriver-human-equipment.kdlb
chara/xls/kinedriver/kinedriver-human-face.kdlb
chara/xls/kinedriver/kinedriver-human-hair.kdlb
chara/xls/kinedriver/kinedriver-monster.kdlb
chara/xls/kinedriver/kinedriver-weapon.kdlb

table TypeIdInfo {
  id:ubyte;
  unk_uint_1:uint;
  has_kdb:bool;
}
table TypeIdArray {
  has_kdbs:bool;
  type_id_info:[TypeIdInfo];
}
table SkeletonIdArray {
  skeleton_id:uint;
  unk_default_1:uint;
  type_id_array:[TypeIdArray];
}


chara/xls/extraskl/extra_weapon.eslb
chara/human/c1701/skeleton/face/f0002/kdi_c1701f0002.kdb

--------------------------------

chara/xls/animation/papLoadTable.plt
chara/xls/boneDeformer/human.pbd
chara/xls/equipmentParameter/equipmentVfxParameter.evp
chara/xls/animation/animation_work_table-demihuman.awt
chara/xls/animation/AnimationExchangeTable.aet
chara/xls/animation/animation_work_table-human.awt
chara/xls/animation/MotionLineTable.mlt
chara/xls/animation/animation_work_table-monster.awt
chara/xls/animation/animation_work_table-weapon.awt

chara/xls/animation_extension/animext.anxb
chara/xls/charadb/extra_met.est
chara/xls/charadb/hairskeletontemplate.est

https://github.com/TexTools/xivModdingFramework/blob/44f0d031d3caa5b813a8c72c08f40313c3029d2e/xivModdingFramework/Models/FileTypes/PDB.cs#L331
https://github.com/TexTools/xivModdingFramework/blob/44f0d031d3caa5b813a8c72c08f40313c3029d2e/xivModdingFramework/Models/Helpers/ModelModifiers.cs#L1207
https://github.com/TexTools/xivModdingFramework/blob/44f0d031d3caa5b813a8c72c08f40313c3029d2e/xivModdingFramework/Models/Helpers/ModelModifiers.cs#L1145
https://github.com/TexTools/xivModdingFramework/blob/44f0d031d3caa5b813a8c72c08f40313c3029d2e/xivModdingFramework/Models/Helpers/ModelModifiers.cs#L1145
```
