// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: WorldDb.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Server.Data.WorldDb {

  /// <summary>Holder for reflection information generated from WorldDb.proto</summary>
  public static partial class WorldDbReflection {

    #region Descriptor
    /// <summary>File descriptor for WorldDb.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static WorldDbReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg1Xb3JsZERiLnByb3RvEhNzZXJ2ZXIuZGF0YS53b3JsZGRiKiIKC1JlcVBh",
            "Y2tldElkEhMKD0RXX1NFUlZFUl9TVEFURRAAKiIKC1Jlc1BhY2tldElkEhMK",
            "D1dEX1NFUlZFUl9TVEFURRAAQhaqAhNTZXJ2ZXIuRGF0YS5Xb3JsZERiYgZw",
            "cm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Server.Data.WorldDb.ReqPacketId), typeof(global::Server.Data.WorldDb.ResPacketId), }, null, null));
    }
    #endregion

  }
  #region Enums
  public enum ReqPacketId {
    [pbr::OriginalName("DW_SERVER_STATE")] DwServerState = 0,
  }

  public enum ResPacketId {
    [pbr::OriginalName("WD_SERVER_STATE")] WdServerState = 0,
  }

  #endregion

}

#endregion Designer generated code
