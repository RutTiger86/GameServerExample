// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: ClientAuth.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Server.Data.ClientAuth {

  /// <summary>Holder for reflection information generated from ClientAuth.proto</summary>
  public static partial class ClientAuthReflection {

    #region Descriptor
    /// <summary>File descriptor for ClientAuth.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ClientAuthReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChBDbGllbnRBdXRoLnByb3RvEhZzZXJ2ZXIuZGF0YS5jbGllbnRhdXRoKiIK",
            "C1JlcVBhY2tldElkEhMKD0NBX1NFUlZFUl9TVEFURRAAKiIKC1Jlc1BhY2tl",
            "dElkEhMKD0FDX1NFUlZFUl9TVEFURRAAQhmqAhZTZXJ2ZXIuRGF0YS5DbGll",
            "bnRBdXRoYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Server.Data.ClientAuth.ReqPacketId), typeof(global::Server.Data.ClientAuth.ResPacketId), }, null, null));
    }
    #endregion

  }
  #region Enums
  public enum ReqPacketId {
    [pbr::OriginalName("CA_SERVER_STATE")] CaServerState = 0,
  }

  public enum ResPacketId {
    [pbr::OriginalName("AC_SERVER_STATE")] AcServerState = 0,
  }

  #endregion

}

#endregion Designer generated code
