using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using TeximpNet;
using TeximpNet.Compression;
using TeximpNet.DDS;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Formats.TextureFormat.CustomTeximpNet;
using VfxEditor.Ui.Export;
using VfxEditor.Utils;

namespace VfxEditor.Formats.TextureFormat.Textures {
    public enum PostConversion {
        None,
        A8,
        R4444,
        R5551,
    }

    public class TextureReplace : TextureDrawable, IFileDocument {
        private string Name = "";
        private string WriteLocation;
        private TexturePreview Preview;

        public TextureReplace( string writeLocation, WorkspaceMetaTex data ) : this( data.ReplacePath, writeLocation ) {
            Name = data.Name ?? "";
        }

        public TextureReplace( string gamePath, string writeLocation ) : base( gamePath ) {
            WriteLocation = writeLocation;
            Plugin.AddCustomBackupLocation( GamePath, WriteLocation );
        }

        public void ImportFile( string importPath ) {
            Preview = null;

            try {
                var importFileExtension = Path.GetExtension( importPath ).ToLower();

                if( importFileExtension == ".dds" ) {
                    var ddsFile = DDSFile.Read( importPath );
                    var format = TextureDataFile.DXGItoTextureFormat( ddsFile.Format );
                    if( format == TextureFormat.Null ) return;

                    using( var writer = new BinaryWriter( File.Open( WriteLocation, FileMode.Create ) ) ) {
                        DdsToAtex( format, File.ReadAllBytes( importPath ), writer );
                    }
                    ddsFile.Dispose();
                }
                else if( importFileExtension == ".atex" || importFileExtension == ".tex" ) {
                    File.Copy( importPath, WriteLocation, true );
                }
                else if( importFileExtension == ".png" ) {
                    using var surface = Surface.LoadFromFile( importPath );
                    surface.FlipVertically();

                    using var compressor = new Compressor();
                    var compFormat = TextureDataFile.TextureToCompressionFormat( Plugin.Configuration.PngFormat );
                    // use ETC1 to signify "NULL" because I'm not going to be using it
                    if( compFormat == CompressionFormat.ETC1 ) return;

                    // Ui elements are required to only have 1 mip level
                    var maxMips = ( ushort )( GamePath.StartsWith( "ui/" ) ? 1 : Plugin.Configuration.PngMips );
                    compressor.Input.SetMipmapGeneration( true, maxMips );
                    compressor.Input.SetData( surface );
                    compressor.Compression.Format = compFormat;
                    compressor.Compression.SetBGRAPixelFormat();
                    compressor.Process( out var ddsContainer );

                    using var ms = new MemoryStream();

                    // lord have mercy
                    CustomDDSFile.Write( ms, ddsContainer.MipChains, ddsContainer.Format, ddsContainer.Dimension );

                    var ddsData = ms.ToArray();

                    using( var writer = new BinaryWriter( File.Open( WriteLocation, FileMode.Create ) ) ) {
                        var postConversion = Plugin.Configuration.PngFormat switch {
                            TextureFormat.A8 => PostConversion.A8,
                            TextureFormat.R4G4B4A4 => PostConversion.R4444,
                            TextureFormat.R5G5B5A1 => PostConversion.R5551,
                            _ => PostConversion.None
                        };

                        DdsToAtex( Plugin.Configuration.PngFormat, ddsData, writer, postConversion );
                    }
                    ddsContainer.Dispose();
                }
                else {
                    throw new Exception( $"Invalid extension {importFileExtension}" );
                }

                Preview = new TexturePreview( TextureDataFile.LoadFromLocal( WriteLocation ), false, GamePath );
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Error importing {importPath} into {GamePath}" );
            }
        }

        public WorkspaceMetaTex WorkspaceExport( string rootPath, int idx ) {
            var extension = Path.GetExtension( WriteLocation );
            var path = $"VFX_{idx}{extension}";
            var fullPath = Path.Combine( rootPath, path );
            File.Copy( WriteLocation, fullPath, true );

            return new WorkspaceMetaTex {
                Name = Name,
                RelativeLocation = path,
                ReplacePath = GamePath
            };
        }

        public bool CanExport() => true;

        // ===========================

        public void DrawBody() {
            var width = ImGui.GetContentRegionAvail().X - 50;

            ImGui.SetNextItemWidth( width );
            ImGui.InputTextWithHint( "Name", TrimPath( GamePath ), ref Name, 255 );

            ImGui.SetNextItemWidth( width );
            ImGui.InputText( "Path", ref GamePath, 255 );

            DrawImage();
            DrawControls();
        }

        public override void DrawFullImage() => Preview?.DrawFullImage();

        public override void DrawImage() => Preview?.DrawImage();

        public override void DrawImage( uint u, uint v, uint w, uint h ) => Preview?.DrawImage( u, v, w, h );

        public override void DrawImage( float height ) => Preview?.DrawImage( height );

        protected override void DrawControls() {
            Preview?.DrawParams();

            ImGui.SameLine();
            using( var color = ImRaii.PushColor( ImGuiCol.Text, UiUtils.PARSED_GREEN ) ) {
                ImGui.Text( "[Replaced]" );
            }

            DrawExportReplaceButtons();

            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( ImGui.GetStyle().ItemInnerSpacing.X, ImGui.GetStyle().ItemSpacing.Y ) );
            ImGui.SameLine();
            if( UiUtils.RemoveButton( "Reset" ) ) Plugin.TextureManager.RemoveReplace( this );
        }

        protected override TextureDataFile GetRawData() => TextureDataFile.LoadFromLocal( WriteLocation );

        protected override TexturePreview GetPreview() => Preview;

        // =========================

        public string GetExportSource() => "";
        public string GetExportReplace() => string.IsNullOrEmpty( Name ) ? TrimPath( GamePath ) : Name;
        public bool IsHd() => GamePath.Contains( "_hr1" );
        public bool Matches( string text ) => GamePath.Contains( text, StringComparison.CurrentCultureIgnoreCase )
            || ( !string.IsNullOrEmpty( Name ) && Name.Contains( text, StringComparison.CurrentCultureIgnoreCase ) );

        public static string TrimPath( string path ) {
            if( string.IsNullOrEmpty( path ) ) return "";

            var split = path.Split( "/" );
            var suffix = split[^1].Split( '.' )[0].Replace( "_hr1", "" );

            if( split.Length < 3 ) return suffix;

            var fallback = $"{split[0]}/{suffix}";

            // bg/ex3/05_zon_z4/common/vfx/texture/t50_dist_001.atex
            // chara/weapon/w1603/obj/body/b0001/vfx/texture/ref_f.atex
            // chara/monster/m0610/obj/body/b0001/vfx/texture/glow001at.atex
            if( split[0] == "bg" || split[0] == "chara" ) return split.Length >= 4 ? $"{split[2]}/{suffix}" : fallback;

            // ui/uld/MiragePrismBoxFilterSetting.uld
            // ui/icon/056000/056512_hr1.tex
            if( split[0] == "ui" ) return fallback;

            // vfx/aoz3/mgc_rod106/texture/tk_aura001n1.atex
            // vfx/cut/qstpdn/qstpdn00250/texture/clsnmt0a060_obi00r.atex
            // vfx/action/ab_2sw_abl029/texture/uvdist01s.atex
            // vfx/ws/wbw_kagenui/texture/ligt019_c.atex
            // vfx/ui/texture/dist_uv_f.atex
            if( split[0] == "vfx" ) {
                if( split[1] == "ui" ) return suffix;
                if( split[1] == "cut" ) return split.Length >= 5 ? $"{split[3]}/{suffix}" : fallback;
                if( split[1] == "custom" ) return split.Length >= 5 ? $"{split[3]}/{suffix}" : fallback;
                return split.Length >= 4 ? $"{split[2]}/{suffix}" : fallback;
            }

            return fallback;
        }

        public bool GetReplacePath( string path, out string replacePath ) {
            replacePath = GamePath.ToLower().Equals( path.ToLower() ) ? WriteLocation : null;
            return !string.IsNullOrEmpty( replacePath );
        }

        public void PenumbraExport( string modFolder, string groupOption, Dictionary<string, string> filesOut ) {
            if( string.IsNullOrEmpty( WriteLocation ) || string.IsNullOrEmpty( GamePath ) ) return;
            PenumbraUtils.CopyFile( WriteLocation, modFolder, groupOption, GamePath, filesOut );
        }

        public void TextoolsExport( BinaryWriter writer, List<TTMPL_Simple> simplePartsOut, ref int modOffset ) {
            if( string.IsNullOrEmpty( WriteLocation ) || string.IsNullOrEmpty( GamePath ) ) return;

            byte[] modData;
            if( GamePath.EndsWith( ".atex" ) ) {
                // Makes Type 2 Data from .atex header + DDS data
                modData = TexToolsUtils.CreateType2Data( File.ReadAllBytes( WriteLocation ) );
            }
            else {
                var file = TextureDataFile.LoadFromLocal( WriteLocation );
                var header = file.Header;

                TextureUtils.GetDdsInfo(
                    file.GetDdsData(), header.Format, header.Width, header.Height, header.MipLevelsCount,
                    out var compressed, out var mipPartOffset, out var mipPartCount );

                using var ms = new MemoryStream();
                using var texWriter = new BinaryWriter( ms );

                // Write Type 4 Data
                texWriter.Write( TextureUtils.CreateType4Data( header.Format, mipPartOffset, mipPartCount, file.GetDdsData().Length,
                        header.MipLevelsCount, header.Width, header.Height ) );

                // Write .atex/.tex header
                var size = Marshal.SizeOf( header );
                var buffer = new byte[size];
                var handle = Marshal.AllocHGlobal( size );
                Marshal.StructureToPtr( header, handle, true );
                Marshal.Copy( handle, buffer, 0, size );
                Marshal.FreeHGlobal( handle );
                texWriter.Write( buffer );

                // Write compressed data
                texWriter.Write( compressed );

                modData = ms.ToArray();
            }

            simplePartsOut.Add( TexToolsUtils.CreateModResource( GamePath, modOffset, modData.Length ) );
            writer.Write( modData );
            modOffset += modData.Length;
        }

        protected override void OnReplace( string importPath ) { // since already replaced, need to refresh it
            WriteLocation = Plugin.TextureManager.GetNewWriteLocation( GamePath );
            Plugin.AddCustomBackupLocation( GamePath, WriteLocation );
            ImportFile( importPath );
        }

        // ===========================

        private static void DdsToAtex( TextureFormat format, byte[] ddsData, BinaryWriter writer, PostConversion post = PostConversion.None ) {
            // Get DDS info
            using var ms = new MemoryStream( ddsData );
            using var reader = new BinaryReader( ms );
            reader.BaseStream.Position = 12;
            var height = reader.ReadInt32();
            var width = reader.ReadInt32();
            var pitch = reader.ReadInt32();
            var depth = reader.ReadInt32();
            var mipLevels = reader.ReadInt32();

            writer.Write( TextureUtils.CreateTextureHeader( format, width, height, mipLevels ) );

            // Add DDS data
            reader.BaseStream.Position = 128;
            var uncompressedLength = ms.Length - 128;
            var imageData = new byte[uncompressedLength];
            reader.Read( imageData, 0, ( int )uncompressedLength );

            // Need to convert from R8G8B8A8
            if( post == PostConversion.A8 ) imageData = TextureDataFile.CompressA8( imageData );

            writer.Write( imageData );
        }
    }
}
