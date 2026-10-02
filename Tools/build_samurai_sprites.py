"""
Build samurai sprite sheets and Unity meta/anim files for the platformer character.
Source: OpenGameArt "Samurai Character" (CC0) by Segel
"""
import glob
import os
import re
import uuid

from PIL import Image

PROJECT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SOURCE = os.path.join(PROJECT, "Assets", "Character", "SamuraiSource", "Samurai1")
SPRITES = os.path.join(PROJECT, "Assets", "Character", "Sprites")
ANIMATIONS = os.path.join(PROJECT, "Assets", "Character", "Animations")

TARGET_HEIGHT = 128
PPU = 100
SAMPLE_RATE = 12

GUIDS = {
    "PlayerIdle": "ba86c7b200abe499cb750833482830b3",
    "PlayerRun": "225a1eacac9dc41ae9a4d6adf7df8d40",
    "PlayerJump": "f43044f34a0b7424eb5deb0c04ff27c7",
    "PlayerLand": "e0d1e122a29a24ec19a2b23782952411",
    "PlayerHurt": "e1c7748eb9afd4e608ce8f923cf861dc",
    "PlayerDeath": "75b38086a902846edbae786183b0274d",
    "PlayerSpawn": "1a4bd6d8b694c4f65869fe4d44a0198b",
    "PlayerVictory": "591161ddccde24e77bc04c29ec2742eb",
}

ANIM_GUIDS = {
    "PlayerIdle": "712f4510fc9b249348d4d8e97b12dc45",
    "PlayerRun": "56811a918221743d69fe84599f43c740",
    "PlayerJump": "1d7e02f8cf5ba47a78bc1e19cc378478",
    "PlayerLand": "9d77f2a0966c94ef3b13bd6bdac0da40",
    "PlayerHurt": "acd3fdc6781e749908cec8f29cbac75f",
    "PlayerDeath": "e0e8baec160e44e1bb74ca8e08777453",
    "PlayerSpawn": "f382f645d530b48e2a2137bfbf095647",
    "PlayerVictory": "e4a08f6d2d50b4e65b8dc54c2429f307",
}

FRAME_SOURCES = {
    "PlayerIdle": os.path.join(SOURCE, "01-Idle", "*.png"),
    "PlayerRun": os.path.join(SOURCE, "02-Run", "*.png"),
    "PlayerJump": [
        os.path.join(SOURCE, "05-Jump&Fall", "__Samurai1_Jump_000.png"),
        os.path.join(SOURCE, "05-Jump&Fall", "__Samurai1_Fall_000.png"),
        os.path.join(SOURCE, "05-Jump&Fall", "__Samurai1_Jump_000.png"),
        os.path.join(SOURCE, "05-Jump&Fall", "__Samurai1_Fall_000.png"),
    ],
    "PlayerLand": [
        os.path.join(SOURCE, "05-Jump&Fall", "__Samurai1_Fall_000.png"),
        os.path.join(SOURCE, "01-Idle", "__Samurai1_Idle_000.png"),
        os.path.join(SOURCE, "01-Idle", "__Samurai1_Idle_001.png"),
    ],
    "PlayerHurt": os.path.join(SOURCE, "06-Hurt", "*.png"),
    "PlayerDeath": os.path.join(SOURCE, "07-Dead", "*.png"),
    "PlayerSpawn": os.path.join(SOURCE, "03-Back", "*.png"),
    "PlayerVictory": os.path.join(SOURCE, "04-Attack", "NoEffect1", "*.png"),
}


def load_frames(pattern_or_list):
    if isinstance(pattern_or_list, list):
        paths = pattern_or_list
    else:
        paths = sorted(glob.glob(pattern_or_list))
    frames = []
    for path in paths:
        img = Image.open(path).convert("RGBA")
        ratio = TARGET_HEIGHT / img.height
        new_w = max(1, int(img.width * ratio))
        img = img.resize((new_w, TARGET_HEIGHT), Image.Resampling.NEAREST)
        frames.append(img)
    return frames


def build_sheet(frames):
    if not frames:
        raise ValueError("No frames to build sheet from")
    fw, fh = frames[0].size
    sheet = Image.new("RGBA", (fw * len(frames), fh), (0, 0, 0, 0))
    for i, frame in enumerate(frames):
        sheet.paste(frame, (i * fw, 0), frame)
    return sheet, fw, fh


def sprite_name(prefix, index):
    return f"Samurai_{prefix}_{index}"


def generate_meta(guid, prefix, frame_w, frame_h, frame_count):
    sprites_yaml = []
    name_table = []
    name_file_id = []

    for i in range(frame_count):
        internal_id = 21300000 + i * 2
        name = sprite_name(prefix, i)
        x = i * frame_w
        name_table.append(f"""  - first:
      213: {internal_id}
    second: {name}""")
        sprites_yaml.append(f"""    - serializedVersion: 2
      name: {name}
      rect:
        serializedVersion: 2
        x: {x}
        y: 0
        width: {frame_w}
        height: {frame_h}
      alignment: 0
      pivot: {{x: 0.5, y: 0.5}}
      border: {{x: 0, y: 0, z: 0, w: 0}}
      outline: []
      physicsShape: []
      tessellationDetail: 0
      bones: []
      spriteID: {uuid.uuid4().hex[:32]}
      internalID: {internal_id}
      vertices: []
      indices: 
      edges: []
      weights: []""")
        name_file_id.append(f"      {name}: {internal_id}")

    return f"""fileFormatVersion: 2
guid: {guid}
TextureImporter:
  internalIDToNameTable:
{chr(10).join(name_table)}
  externalObjects: {{}}
  serializedVersion: 11
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
  isReadable: 0
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMasterTextureLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: 2
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {{x: 0.5, y: 0.5}}
  spritePixelsToUnits: {PPU}
  spriteBorder: {{x: 0, y: 0, z: 0, w: 0}}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 1
  platformSettings:
  - serializedVersion: 3
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 0
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 1
  - serializedVersion: 3
    buildTarget: Standalone
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 0
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 1
  spriteSheet:
    serializedVersion: 2
    sprites:
{chr(10).join(sprites_yaml)}
    outline: []
    physicsShape: []
    bones: []
    spriteID: {uuid.uuid4().hex[:32]}
    internalID: 0
    vertices: []
    indices: 
    edges: []
    weights: []
    secondaryTextures: []
    nameFileIdTable:
{chr(10).join(name_file_id)}
  spritePackingTag: 
  pSDRemoveMatte: 0
  pSDShowRemoveMatteOption: 0
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def generate_anim(anim_name, texture_guid, frame_count, loop=True):
    frame_time = 1.0 / SAMPLE_RATE
    curve_lines = []
    mapping_lines = []

    for i in range(frame_count):
        internal_id = 21300000 + i * 2
        t = round(i * frame_time, 8)
        ref = f"{{fileID: {internal_id}, guid: {texture_guid}, type: 3}}"
        curve_lines.append(f"    - time: {t}")
        curve_lines.append(f"      value: {ref}")
        mapping_lines.append(f"    - {ref}")

    stop_time = round((frame_count - 1) * frame_time + frame_time, 8)

    return f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!74 &7400000
AnimationClip:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_Name: {anim_name}
  serializedVersion: 6
  m_Legacy: 0
  m_Compressed: 0
  m_UseHighQualityCurve: 1
  m_RotationCurves: []
  m_CompressedRotationCurves: []
  m_EulerCurves: []
  m_PositionCurves: []
  m_ScaleCurves: []
  m_FloatCurves: []
  m_PPtrCurves:
  - curve:
{chr(10).join(curve_lines)}
    attribute: m_Sprite
    path: 
    classID: 212
    script: {{fileID: 0}}
  m_SampleRate: {SAMPLE_RATE}
  m_WrapMode: 0
  m_Bounds:
    m_Center: {{x: 0, y: 0, z: 0}}
    m_Extent: {{x: 0, y: 0, z: 0}}
  m_ClipBindingConstant:
    genericBindings:
    - serializedVersion: 2
      path: 0
      attribute: 0
      script: {{fileID: 0}}
      typeID: 212
      customType: 23
      isPPtrCurve: 1
    pptrCurveMapping:
{chr(10).join(mapping_lines)}
  m_AnimationClipSettings:
    serializedVersion: 2
    m_AdditiveReferencePoseClip: {{fileID: 0}}
    m_AdditiveReferencePoseTime: 0
    m_StartTime: 0
    m_StopTime: {stop_time}
    m_OrientationOffsetY: 0
    m_Level: 0
    m_CycleOffset: 0
    m_HasAdditiveReferencePose: 0
    m_LoopTime: {1 if loop else 0}
    m_LoopBlend: 0
    m_LoopBlendOrientation: 0
    m_LoopBlendPositionY: 0
    m_LoopBlendPositionXZ: 0
    m_KeepOriginalOrientation: 0
    m_KeepOriginalPositionY: 1
    m_KeepOriginalPositionXZ: 0
    m_HeightFromFeet: 0
    m_Mirror: 0
  m_EditorCurves: []
  m_EulerEditorCurves: []
  m_HasGenericRootTransform: 0
  m_HasMotionFloatCurves: 0
  m_Events: []
"""


def main():
    os.makedirs(SPRITES, exist_ok=True)
    os.makedirs(ANIMATIONS, exist_ok=True)

    for name, source in FRAME_SOURCES.items():
        prefix = name.replace("Player", "").lower()
        frames = load_frames(source)
        sheet, fw, fh = build_sheet(frames)
        png_path = os.path.join(SPRITES, f"{name}.png")
        meta_path = png_path + ".meta"
        anim_path = os.path.join(ANIMATIONS, f"{name}.anim")

        sheet.save(png_path)
        with open(meta_path, "w", encoding="utf-8", newline="\n") as f:
            f.write(generate_meta(GUIDS[name], prefix, fw, fh, len(frames)))

        loop = name not in ("PlayerLand", "PlayerHurt", "PlayerDeath", "PlayerSpawn", "PlayerVictory")
        with open(anim_path, "w", encoding="utf-8", newline="\n") as f:
            f.write(generate_anim(name, GUIDS[name], len(frames), loop=loop))

        print(f"Built {name}: {len(frames)} frames, {fw}x{fh}px each")

    print("Done! Open Unity to see the samurai character.")


if __name__ == "__main__":
    main()
