using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using VFXEditor.Helper;
using VFXEditor.Texture;

namespace VFXEditor.Dialogs {
    public class ToolsDialogUtilitiesTab {
        private string RawInputValue = "";
        private string RawTexInputValue = "";

        public void Draw() {
            ImGui.Text( "Extract a raw .avfx file" );
            ImGui.InputText( "Path##RawExtract", ref RawInputValue, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "Extract##RawExtract" ) ) {
                var result = Plugin.DataManager.FileExists( RawInputValue );
                if( result ) {
                    try {
                        var file = Plugin.DataManager.GetFile( RawInputValue );
                        UIHelper.WriteBytesDialog( ".avfx", file.Data, "avfx" );
                    }
                    catch( Exception e ) {
                        PluginLog.Error( "Could not read file", e );
                    }
                }
            }

            ImGui.Text( "Extract an .atex file" );
            ImGui.InputText( "Path##RawTexExtract", ref RawTexInputValue, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "Extract##RawTexExtract" ) ) {
                var result = Plugin.DataManager.FileExists( RawTexInputValue );
                if( result ) {
                    try {
                        var file = Plugin.DataManager.GetFile( RawTexInputValue );
                        UIHelper.WriteBytesDialog( ".atex", file.Data, "atex" );
                    }
                    catch( Exception e ) {
                        PluginLog.Error( e, "Could not read file" );
                    }
                }
            }

            ImGui.Text( ".atex to PNG" );
            ImGui.SameLine();
            if( ImGui.Button( "Browse##AtexToPNG" ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", ".atex,.*", ( bool ok, string res ) => {
                    if( !ok ) return;
                    var texFile = VFXTexture.LoadFromLocal( res );
                    texFile.SaveAsPNG( res + ".png" );
                } );
            }

            ImGui.Text( ".atex to DDS" );
            ImGui.SameLine();
            if( ImGui.Button( "Browse##AtexToDDS" ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", ".atex,.*", ( bool ok, string res ) => {
                    if( !ok ) return;
                    var texFile = VFXTexture.LoadFromLocal( res );
                    texFile.SaveAsDDS( res + ".dds" );
                } );
            }
        }
    }
}