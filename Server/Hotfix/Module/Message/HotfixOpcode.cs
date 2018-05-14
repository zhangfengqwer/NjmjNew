namespace ETModel
{
	public static partial class HotfixOpcode
	{
		 public const ushort C2R_PhoneLogin = 10001;
		 public const ushort R2C_PhoneLogin = 10002;
		 public const ushort C2R_SendSms = 10003;
		 public const ushort R2C_SendSms = 10004;
		 public const ushort C2R_Login = 10005;
		 public const ushort R2C_Login = 10006;
		 public const ushort C2R_Register = 10007;
		 public const ushort R2C_Register = 10008;
		 public const ushort C2G_LoginGate = 10009;
		 public const ushort G2C_LoginGate = 10010;
		 public const ushort G2C_TestHotfixMessage = 10011;
		 public const ushort C2M_TestActorRequest = 10012;
		 public const ushort M2C_TestActorResponse = 10013;
		 public const ushort PlayerInfo = 10014;
		 public const ushort ShopInfo = 10015;
		 public const ushort TaskProgress = 10016;
		 public const ushort C2G_UpdateTaskProgress = 10017;
		 public const ushort G2C_UpdateTaskProgress = 10018;
		 public const ushort C2G_Task = 10019;
		 public const ushort G2C_Task = 10020;
		 public const ushort TaskInfo = 10021;
		 public const ushort C2G_PlayerInfo = 10022;
		 public const ushort G2C_PlayerInfo = 10023;
		 public const ushort C2G_EnterRoom = 10024;
		 public const ushort G2C_EnterRoom = 10025;
		 public const ushort GamerInfo = 10026;
		 public const ushort G2M_PlayerEnterRoom = 10027;
		 public const ushort M2G_PlayerEnterRoom = 10028;
		 public const ushort Actor_GamerEnterRoom = 10029;
		 public const ushort G2M_PlayerExitRoom = 10030;
		 public const ushort M2G_PlayerExitRoom = 10031;
		 public const ushort C2G_UpdatePlayerInfo = 10032;
		 public const ushort G2C_UpdatePlayerInfo = 10033;
		 public const ushort C2M_ActorGamerEnterRoom = 10034;
		 public const ushort M2C_ActorGamerEnterRoom = 10035;
		 public const ushort Actor_GamerExitRoom = 10036;
		 public const ushort M2G_Actor_GamerExitRoom = 10037;
		 public const ushort Actor_GamerReady = 10038;
		 public const ushort Actor_StartGame = 10039;
		 public const ushort Actor_ChangeTable = 10040;
		 public const ushort Email = 10041;
		 public const ushort C2G_Email = 10042;
		 public const ushort G2C_Email = 10043;
		 public const ushort C2G_UpdateEmail = 10044;
		 public const ushort G2C_UpdateEmail = 10045;
	}
}
