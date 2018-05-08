namespace ETModel
{
	public static partial class HotfixOpcode
	{
		 public const ushort C2R_Login = 10001;
		 public const ushort R2C_Login = 10002;
		 public const ushort C2R_Register = 10003;
		 public const ushort R2C_Register = 10004;
		 public const ushort C2G_LoginGate = 10005;
		 public const ushort G2C_LoginGate = 10006;
		 public const ushort G2C_TestHotfixMessage = 10007;
		 public const ushort C2M_TestActorRequest = 10008;
		 public const ushort M2C_TestActorResponse = 10009;
		 public const ushort PlayerInfo = 10010;
		 public const ushort C2G_PlayerInfo = 10011;
		 public const ushort G2C_PlayerInfo = 10012;
		 public const ushort C2G_EnterRoom = 10013;
		 public const ushort G2C_EnterRoom = 10014;
		 public const ushort GamerInfo = 10015;
		 public const ushort G2M_PlayerEnterRoom = 10016;
		 public const ushort M2G_PlayerEnterRoom = 10017;
		 public const ushort Actor_GamerEnterRoom = 10018;
		 public const ushort C2G_UpdatePlayerInfo = 10019;
		 public const ushort G2C_UpdatePlayerInfo = 10020;
	}
}
