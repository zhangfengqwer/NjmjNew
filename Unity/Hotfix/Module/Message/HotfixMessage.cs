using ProtoBuf;
using ETModel;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
namespace ETHotfix
{
	[Message(HotfixOpcode.C2R_PhoneLogin)]
	[ProtoContract]
	public partial class C2R_PhoneLogin: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Phone;

		[ProtoMember(2, IsRequired = true)]
		public string Code;

		[ProtoMember(3, IsRequired = true)]
		public string Token;

		[ProtoMember(4, IsRequired = true)]
		public string MachineId;

		[ProtoMember(5, IsRequired = true)]
		public string ChannelName;

		[ProtoMember(6, IsRequired = true)]
		public string ClientVersion;

	}

	[Message(HotfixOpcode.R2C_PhoneLogin)]
	[ProtoContract]
	public partial class R2C_PhoneLogin: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Address;

		[ProtoMember(2, IsRequired = true)]
		public long Key;

		[ProtoMember(3, IsRequired = true)]
		public string Token;

	}

	[Message(HotfixOpcode.C2R_ThirdLogin)]
	[ProtoContract]
	public partial class C2R_ThirdLogin: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Third_Id;

		[ProtoMember(2, IsRequired = true)]
		public string MachineId;

		[ProtoMember(3, IsRequired = true)]
		public string ChannelName;

		[ProtoMember(4, IsRequired = true)]
		public string ClientVersion;

		[ProtoMember(5, IsRequired = true)]
		public string Name;

		[ProtoMember(6, IsRequired = true)]
		public string Head;

	}

	[Message(HotfixOpcode.R2C_ThirdLogin)]
	[ProtoContract]
	public partial class R2C_ThirdLogin: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Address;

		[ProtoMember(2, IsRequired = true)]
		public long Key;

	}

	[Message(HotfixOpcode.C2R_ChangeAccount)]
	[ProtoContract]
	public partial class C2R_ChangeAccount: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

	}

	[Message(HotfixOpcode.R2C_ChangeAccount)]
	[ProtoContract]
	public partial class R2C_ChangeAccount: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

	[Message(HotfixOpcode.Actor_ForceOffline)]
	[ProtoContract]
	public partial class Actor_ForceOffline: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

	}

	[Message(HotfixOpcode.C2R_SendSms)]
	[ProtoContract]
	public partial class C2R_SendSms: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Phone;

	}

	[Message(HotfixOpcode.R2C_SendSms)]
	[ProtoContract]
	public partial class R2C_SendSms: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

	[Message(HotfixOpcode.C2R_Login)]
	[ProtoContract]
	public partial class C2R_Login: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Account;

		[ProtoMember(2, IsRequired = true)]
		public string Password;

	}

	[Message(HotfixOpcode.R2C_Login)]
	[ProtoContract]
	public partial class R2C_Login: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Address;

		[ProtoMember(2, IsRequired = true)]
		public long Key;

	}

	[Message(HotfixOpcode.C2R_Register)]
	[ProtoContract]
	public partial class C2R_Register: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Account;

		[ProtoMember(2, IsRequired = true)]
		public string Password;

	}

	[Message(HotfixOpcode.R2C_Register)]
	[ProtoContract]
	public partial class R2C_Register: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

	[Message(HotfixOpcode.C2G_LoginGate)]
	[ProtoContract]
	public partial class C2G_LoginGate: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Key;

	}

	[Message(HotfixOpcode.G2C_LoginGate)]
	[ProtoContract]
	public partial class G2C_LoginGate: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long PlayerId;

		[ProtoMember(2, IsRequired = true)]
		public long Uid;

		[ProtoMember(3, TypeName = "ETHotfix.ShopInfo")]
		public List<ShopInfo> ShopInfoList = new List<ShopInfo>();

		[ProtoMember(4, TypeName = "ETHotfix.TaskInfo")]
		public List<TaskInfo> TaskInfoList = new List<TaskInfo>();

		[ProtoMember(5, TypeName = "ETHotfix.Bag")]
		public List<Bag> BagList = new List<Bag>();

		[ProtoMember(6, IsRequired = true)]
		public string ownIcon;

	}

	[Message(HotfixOpcode.G2C_TestHotfixMessage)]
	[ProtoContract]
	public partial class G2C_TestHotfixMessage: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public string Info;

	}

	[Message(HotfixOpcode.C2M_TestActorRequest)]
	[ProtoContract]
	public partial class C2M_TestActorRequest: IActorRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Info;

	}

	[Message(HotfixOpcode.M2C_TestActorResponse)]
	[ProtoContract]
	public partial class M2C_TestActorResponse: IActorResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Info;

	}

	[Message(HotfixOpcode.PlayerInfo)]
	[ProtoContract]
	public partial class PlayerInfo: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public string Name;

		[ProtoMember(2, IsRequired = true)]
		public long GoldNum;

		[ProtoMember(3, IsRequired = true)]
		public long WingNum;

		[ProtoMember(4, IsRequired = true)]
		public string Icon;

		[ProtoMember(5, IsRequired = true)]
		public bool IsRealName;

		[ProtoMember(6, IsRequired = true)]
		public string Phone;

		[ProtoMember(7, IsRequired = true)]
		public int RestChangeNameCount;

		[ProtoMember(8, IsRequired = true)]
		public int TotalGameCount;

		[ProtoMember(9, IsRequired = true)]
		public int WinGameCount;

		[ProtoMember(10, IsRequired = true)]
		public int PlayerSound;

		[ProtoMember(11, IsRequired = true)]
		public string VipTime;

		[ProtoMember(12, IsRequired = true)]
		public int HuaFeiNum;

		[ProtoMember(13, IsRequired = true)]
		public string EmogiTime;

		[ProtoMember(14, IsRequired = true)]
		public int MaxHua;

		[ProtoMember(15, IsRequired = true)]
		public bool IsSign;

	}

	[Message(HotfixOpcode.PlayerIcon)]
	[ProtoContract]
	public partial class PlayerIcon: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public string Icon;

	}

	[Message(HotfixOpcode.ShopInfo)]
	[ProtoContract]
	public partial class ShopInfo: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public long Id;

		[ProtoMember(2, IsRequired = true)]
		public int ShopType;

		[ProtoMember(3, IsRequired = true)]
		public string Name;

		[ProtoMember(4, IsRequired = true)]
		public string Desc;

		[ProtoMember(5, IsRequired = true)]
		public int Price;

		[ProtoMember(6, IsRequired = true)]
		public int CurrencyType;

		[ProtoMember(7, IsRequired = true)]
		public string Items;

		[ProtoMember(8, IsRequired = true)]
		public string Icon;

		[ProtoMember(9, IsRequired = true)]
		public int VipPrice;

	}

	[Message(HotfixOpcode.GetItemInfo)]
	[ProtoContract]
	public partial class GetItemInfo: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public int ItemID;

		[ProtoMember(2, IsRequired = true)]
		public int Count;

	}

	[Message(HotfixOpcode.C2G_BuyItem)]
	[ProtoContract]
	public partial class C2G_BuyItem: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UId;

		[ProtoMember(2, IsRequired = true)]
		public GetItemInfo Info;

		[ProtoMember(3, IsRequired = true)]
		public int CurrencyType;

		[ProtoMember(4, IsRequired = true)]
		public int Cost;

	}

	[Message(HotfixOpcode.Actor_UpDateData)]
	[ProtoContract]
	public partial class Actor_UpDateData: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public PlayerInfo playerInfo;

	}

	[Message(HotfixOpcode.G2C_BuyItem)]
	[ProtoContract]
	public partial class G2C_BuyItem: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Count;

		[ProtoMember(2, IsRequired = true)]
		public long Wealth;

		[ProtoMember(3, IsRequired = true)]
		public int CurrencyType;

	}

	[Message(HotfixOpcode.C2G_GetTaskReward)]
	[ProtoContract]
	public partial class C2G_GetTaskReward: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UId;

		[ProtoMember(2, IsRequired = true)]
		public TaskInfo TaskInfo;

		[ProtoMember(3, IsRequired = true)]
		public int GetType;

	}

	[Message(HotfixOpcode.G2C_GetTaskReward)]
	[ProtoContract]
	public partial class G2C_GetTaskReward: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

	[Message(HotfixOpcode.C2G_Task)]
	[ProtoContract]
	public partial class C2G_Task: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long uid;

	}

	[Message(HotfixOpcode.G2C_Task)]
	[ProtoContract]
	public partial class G2C_Task: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, TypeName = "ETHotfix.TaskInfo")]
		public List<TaskInfo> TaskProgressList = new List<TaskInfo>();

	}

	[Message(HotfixOpcode.C2G_Chengjiu)]
	[ProtoContract]
	public partial class C2G_Chengjiu: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

	}

	[Message(HotfixOpcode.G2C_Chengjiu)]
	[ProtoContract]
	public partial class G2C_Chengjiu: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, TypeName = "ETHotfix.TaskInfo")]
		public List<TaskInfo> ChengjiuList = new List<TaskInfo>();

	}

	[Message(HotfixOpcode.TaskInfo)]
	[ProtoContract]
	public partial class TaskInfo: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public int Id;

		[ProtoMember(2, IsRequired = true)]
		public string TaskName;

		[ProtoMember(3, IsRequired = true)]
		public string Desc;

		[ProtoMember(4, IsRequired = true)]
		public int Reward;

		[ProtoMember(5, IsRequired = true)]
		public int Target;

		[ProtoMember(6, IsRequired = true)]
		public int Progress;

		[ProtoMember(7, IsRequired = true)]
		public bool IsComplete;

		[ProtoMember(8, IsRequired = true)]
		public bool IsGet;

	}

	[Message(HotfixOpcode.C2G_PlayerInfo)]
	[ProtoContract]
	public partial class C2G_PlayerInfo: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long uid;

	}

	[Message(HotfixOpcode.G2C_PlayerInfo)]
	[ProtoContract]
	public partial class G2C_PlayerInfo: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public PlayerInfo PlayerInfo;

		[ProtoMember(2, IsRequired = true)]
		public string OwnIcon;

	}

	[Message(HotfixOpcode.Activity)]
	[ProtoContract]
	public partial class Activity: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public long UId;

		[ProtoMember(2, IsRequired = true)]
		public int ActivityId;

		[ProtoMember(3, IsRequired = true)]
		public string Title;

	}

	[Message(HotfixOpcode.C2G_Activity)]
	[ProtoContract]
	public partial class C2G_Activity: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UId;

	}

	[Message(HotfixOpcode.G2C_Activity)]
	[ProtoContract]
	public partial class G2C_Activity: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, TypeName = "ETHotfix.Activity")]
		public List<Activity> activityList = new List<Activity>();

	}

	[Message(HotfixOpcode.G2C_UseItem)]
	[ProtoContract]
	public partial class G2C_UseItem: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public int result;

		[ProtoMember(2, IsRequired = true)]
		public string time;

		[ProtoMember(3, IsRequired = true)]
		public string reward;

	}

	[Message(HotfixOpcode.C2G_UseItem)]
	[ProtoContract]
	public partial class C2G_UseItem: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UId;

		[ProtoMember(2, IsRequired = true)]
		public int ItemId;

	}

	[Message(HotfixOpcode.C2G_ChangeWealth)]
	[ProtoContract]
	public partial class C2G_ChangeWealth: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UId;

		[ProtoMember(2, IsRequired = true)]
		public int propId;

		[ProtoMember(3, IsRequired = true)]
		public int propNum;

	}

	[Message(HotfixOpcode.G2C_ChangeWealth)]
	[ProtoContract]
	public partial class G2C_ChangeWealth: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

	[Message(HotfixOpcode.C2G_EnterRoom)]
	[ProtoContract]
	public partial class C2G_EnterRoom: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public int RoomType;

	}

	[Message(HotfixOpcode.G2C_EnterRoom)]
	[ProtoContract]
	public partial class G2C_EnterRoom: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

	[Message(HotfixOpcode.GamerInfo)]
	[ProtoContract]
	public partial class GamerInfo: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public long UserID;

		[ProtoMember(2, IsRequired = true)]
		public bool IsReady;

		[ProtoMember(3, IsRequired = true)]
		public int SeatIndex;

		[ProtoMember(4, IsRequired = true)]
		public PlayerInfo playerInfo;

	}

	[Message(HotfixOpcode.G2M_PlayerEnterRoom)]
	[ProtoContract]
	public partial class G2M_PlayerEnterRoom: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UserId;

		[ProtoMember(2, IsRequired = true)]
		public long SessionId;

		[ProtoMember(3, IsRequired = true)]
		public long PlayerId;

		[ProtoMember(4, IsRequired = true)]
		public int RoomType;

	}

	[Message(HotfixOpcode.M2G_PlayerEnterRoom)]
	[ProtoContract]
	public partial class M2G_PlayerEnterRoom: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long GameId;

	}

	[Message(HotfixOpcode.Actor_GamerEnterRoom)]
	[ProtoContract]
	public partial class Actor_GamerEnterRoom: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, TypeName = "ETHotfix.GamerInfo")]
		public List<GamerInfo> Gamers = new List<GamerInfo>();

		[ProtoMember(2, IsRequired = true)]
		public int RoomType;

	}

	[Message(HotfixOpcode.Actor_GamerJionRoom)]
	[ProtoContract]
	public partial class Actor_GamerJionRoom: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public GamerInfo Gamer;

	}

	[Message(HotfixOpcode.Actor_GamerBuyYuanBao)]
	[ProtoContract]
	public partial class Actor_GamerBuyYuanBao: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public int goodsId;

	}

	[Message(HotfixOpcode.G2M_PlayerExitRoom)]
	[ProtoContract]
	public partial class G2M_PlayerExitRoom: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UserId;

		[ProtoMember(2, IsRequired = true)]
		public long GameId;

	}

	[Message(HotfixOpcode.M2G_PlayerExitRoom)]
	[ProtoContract]
	public partial class M2G_PlayerExitRoom: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long GameId;

	}

	[Message(HotfixOpcode.C2G_UpdatePlayerInfo)]
	[ProtoContract]
	public partial class C2G_UpdatePlayerInfo: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public PlayerInfo playerInfo;

	}

	[Message(HotfixOpcode.G2C_UpdatePlayerInfo)]
	[ProtoContract]
	public partial class G2C_UpdatePlayerInfo: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public PlayerInfo playerInfo;

	}

	[Message(HotfixOpcode.C2G_UpdateChengjiu)]
	[ProtoContract]
	public partial class C2G_UpdateChengjiu: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UId;

		[ProtoMember(2, IsRequired = true)]
		public TaskInfo TaskPrg;

	}

	[Message(HotfixOpcode.G2C_UpdateChengjiu)]
	[ProtoContract]
	public partial class G2C_UpdateChengjiu: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public TaskInfo TaskPrg;

	}

	[Message(HotfixOpcode.C2M_ActorGamerEnterRoom)]
	[ProtoContract]
	public partial class C2M_ActorGamerEnterRoom: IActorRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Info;

	}

	[Message(HotfixOpcode.M2C_ActorGamerGetTreasure)]
	[ProtoContract]
	public partial class M2C_ActorGamerGetTreasure: IActorResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public int RestSeconds;

		[ProtoMember(2, IsRequired = true)]
		public int Reward;

	}

	[Message(HotfixOpcode.C2M_ActorGamerGetTreasure)]
	[ProtoContract]
	public partial class C2M_ActorGamerGetTreasure: IActorRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

	}

	[Message(HotfixOpcode.M2C_ActorGamerEnterRoom)]
	[ProtoContract]
	public partial class M2C_ActorGamerEnterRoom: IActorResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Info;

	}

	[Message(HotfixOpcode.Actor_GamerExitRoom)]
	[ProtoContract]
	public partial class Actor_GamerExitRoom: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public bool IsFromClient;

	}

	[Message(HotfixOpcode.Actor_GamerChangeGold)]
	[ProtoContract]
	public partial class Actor_GamerChangeGold: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public int GoldAmount;

	}

	[Message(HotfixOpcode.Actor_GamerCheat)]
	[ProtoContract]
	public partial class Actor_GamerCheat: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public string Info;

		[ProtoMember(3, TypeName = "ETHotfix.MahjongInfo")]
		public List<MahjongInfo> handCards = new List<MahjongInfo>();

	}

	[Message(HotfixOpcode.Actor_GameFlow)]
	[ProtoContract]
	public partial class Actor_GameFlow: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

	}

	[Message(HotfixOpcode.Actor_GamerContinueGame)]
	[ProtoContract]
	public partial class Actor_GamerContinueGame: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

	}

	[Message(HotfixOpcode.M2G_Actor_GamerExitRoom)]
	[ProtoContract]
	public partial class M2G_Actor_GamerExitRoom: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

	}

	[Message(HotfixOpcode.Actor_GamerReady)]
	[ProtoContract]
	public partial class Actor_GamerReady: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

	}

	[Message(HotfixOpcode.Actor_GamerReadyTimeOut)]
	[ProtoContract]
	public partial class Actor_GamerReadyTimeOut: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public string Message;

	}

	[Message(HotfixOpcode.Actor_StartGame)]
	[ProtoContract]
	public partial class Actor_StartGame: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, TypeName = "ETHotfix.GamerData")]
		public List<GamerData> GamerDatas = new List<GamerData>();

		[ProtoMember(2, IsRequired = true)]
		public int restCount;

		[ProtoMember(3, IsRequired = true)]
		public int RoomType;

	}

	[Message(HotfixOpcode.GamerData)]
	[ProtoContract]
	public partial class GamerData: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public long UserID;

		[ProtoMember(2, IsRequired = true)]
		public int SeatIndex;

		[ProtoMember(3, TypeName = "ETHotfix.MahjongInfo")]
		public List<MahjongInfo> handCards = new List<MahjongInfo>();

		[ProtoMember(4, TypeName = "ETHotfix.MahjongInfo")]
		public List<MahjongInfo> faceCards = new List<MahjongInfo>();

		[ProtoMember(5, TypeName = "ETHotfix.MahjongInfo")]
		public List<MahjongInfo> playCards = new List<MahjongInfo>();

		[ProtoMember(6, TypeName = "ETHotfix.MahjongInfo")]
		public List<MahjongInfo> pengCards = new List<MahjongInfo>();

		[ProtoMember(7, TypeName = "ETHotfix.MahjongInfo")]
		public List<MahjongInfo> gangCards = new List<MahjongInfo>();

		[ProtoMember(8, IsRequired = true)]
		public bool IsBanker;

		[ProtoMember(9, IsRequired = true)]
		public int OnlineSeconds;

		[ProtoMember(10, IsRequired = true)]
		public PlayerInfo playerInfo;

	}

	[Message(HotfixOpcode.Actor_ChangeTable)]
	[ProtoContract]
	public partial class Actor_ChangeTable: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = false)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public int RoomType;

	}

	[Message(HotfixOpcode.Actor_GamerPlayCard)]
	[ProtoContract]
	public partial class Actor_GamerPlayCard: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public int weight;

		[ProtoMember(2, IsRequired = false)]
		public long Uid;

		[ProtoMember(3, IsRequired = true)]
		public int index;

	}

	[Message(HotfixOpcode.Actor_GamerGrabCard)]
	[ProtoContract]
	public partial class Actor_GamerGrabCard: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public int weight;

		[ProtoMember(2, IsRequired = true)]
		public long Uid;

	}

	[Message(HotfixOpcode.Actor_GamerBuHua)]
	[ProtoContract]
	public partial class Actor_GamerBuHua: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public int weight;

		[ProtoMember(2, IsRequired = true)]
		public long Uid;

	}

	[Message(HotfixOpcode.Actor_GamerOperation)]
	[ProtoContract]
	public partial class Actor_GamerOperation: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = false)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public int OperationType;

		[ProtoMember(3, IsRequired = true)]
		public int weight;

	}

	[Message(HotfixOpcode.Actor_GamerCanOperation)]
	[ProtoContract]
	public partial class Actor_GamerCanOperation: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = false)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public int OperationType;

	}

	[Message(HotfixOpcode.Actor_GamerHuPai)]
	[ProtoContract]
	public partial class Actor_GamerHuPai: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public bool IsZiMo;

		[ProtoMember(3, TypeName = "ETHotfix.GamerData")]
		public List<GamerData> GamerDatas = new List<GamerData>();

		[ProtoMember(4, IsRequired = true)]
		public int YingHuaCount;

		[ProtoMember(5, IsRequired = true)]
		public int RuanHuaCount;

		[ProtoMember(6, TypeName = "ETHotfix.int")]
		public List<int> HuPaiTypes = new List<int>();

		[ProtoMember(7, IsRequired = false)]
		public long FangPaoUid;

		[ProtoMember(8, IsRequired = true)]
		public int BixiaHuCount;

	}

	[Message(HotfixOpcode.Actor_GamerReconnet)]
	[ProtoContract]
	public partial class Actor_GamerReconnet: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, TypeName = "ETHotfix.GamerData")]
		public List<GamerData> Gamers = new List<GamerData>();

		[ProtoMember(2, IsRequired = true)]
		public int RestCount;

		[ProtoMember(3, IsRequired = true)]
		public int RoomType;

	}

	[Message(HotfixOpcode.Actor_GamerTrusteeship)]
	[ProtoContract]
	public partial class Actor_GamerTrusteeship: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

	}

	[Message(HotfixOpcode.Actor_GamerCancelTrusteeship)]
	[ProtoContract]
	public partial class Actor_GamerCancelTrusteeship: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

	}

	[Message(HotfixOpcode.Email)]
	[ProtoContract]
	public partial class Email: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public string EmailTitle;

		[ProtoMember(2, IsRequired = true)]
		public string Content;

		[ProtoMember(3, IsRequired = true)]
		public string Date;

		[ProtoMember(4, IsRequired = true)]
		public int State;

		[ProtoMember(5, IsRequired = true)]
		public string RewardItem;

		[ProtoMember(6, IsRequired = true)]
		public int EId;

	}

	[Message(HotfixOpcode.C2G_EmailOperate)]
	[ProtoContract]
	public partial class C2G_EmailOperate: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public int EmailId;

		[ProtoMember(3, TypeName = "ETHotfix.GetItemInfo")]
		public List<GetItemInfo> InfoList = new List<GetItemInfo>();

		[ProtoMember(4, IsRequired = true)]
		public int state;

	}

	[Message(HotfixOpcode.G2C_EmailOperate)]
	[ProtoContract]
	public partial class G2C_EmailOperate: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

	[Message(HotfixOpcode.C2G_GetItem)]
	[ProtoContract]
	public partial class C2G_GetItem: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UId;

		[ProtoMember(2, TypeName = "ETHotfix.GetItemInfo")]
		public List<GetItemInfo> InfoList = new List<GetItemInfo>();

		[ProtoMember(3, IsRequired = true)]
		public long MailId;

	}

	[Message(HotfixOpcode.G2C_GetItem)]
	[ProtoContract]
	public partial class G2C_GetItem: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public bool Result;

	}

	[Message(HotfixOpcode.C2G_Email)]
	[ProtoContract]
	public partial class C2G_Email: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

	}

	[Message(HotfixOpcode.G2C_Email)]
	[ProtoContract]
	public partial class G2C_Email: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, TypeName = "ETHotfix.Email")]
		public List<Email> EmailInfoList = new List<Email>();

	}

	[Message(HotfixOpcode.Bag)]
	[ProtoContract]
	public partial class Bag: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public int ItemId;

		[ProtoMember(2, IsRequired = true)]
		public int Count;

	}

	[Message(HotfixOpcode.C2G_BagOperation)]
	[ProtoContract]
	public partial class C2G_BagOperation: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UId;

	}

	[Message(HotfixOpcode.G2C_BagOperation)]
	[ProtoContract]
	public partial class G2C_BagOperation: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, TypeName = "ETHotfix.Bag")]
		public List<Bag> ItemList = new List<Bag>();

	}

	[Message(HotfixOpcode.C2G_UpdateEmail)]
	[ProtoContract]
	public partial class C2G_UpdateEmail: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long EId;

		[ProtoMember(2, IsRequired = true)]
		public bool IsRead;

	}

	[Message(HotfixOpcode.G2C_UpdateEmail)]
	[ProtoContract]
	public partial class G2C_UpdateEmail: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long EId;

	}

	[Message(HotfixOpcode.WealthRank)]
	[ProtoContract]
	public partial class WealthRank: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public string PlayerName;

		[ProtoMember(2, IsRequired = true)]
		public long GoldNum;

		[ProtoMember(3, IsRequired = true)]
		public long GoldTicket;

		[ProtoMember(4, IsRequired = true)]
		public string Icon;

		[ProtoMember(5, IsRequired = true)]
		public long UId;

	}

	[Message(HotfixOpcode.GameRank)]
	[ProtoContract]
	public partial class GameRank: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public string PlayerName;

		[ProtoMember(2, IsRequired = true)]
		public int WinCount;

		[ProtoMember(3, IsRequired = true)]
		public int TotalCount;

		[ProtoMember(4, IsRequired = true)]
		public string Icon;

		[ProtoMember(5, IsRequired = true)]
		public long UId;

	}

	[Message(HotfixOpcode.C2G_Rank)]
	[ProtoContract]
	public partial class C2G_Rank: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public int RankType;

	}

	[Message(HotfixOpcode.G2C_Rank)]
	[ProtoContract]
	public partial class G2C_Rank: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, TypeName = "ETHotfix.WealthRank")]
		public List<WealthRank> RankList = new List<WealthRank>();

		[ProtoMember(2, TypeName = "ETHotfix.GameRank")]
		public List<GameRank> GameRankList = new List<GameRank>();

		[ProtoMember(3, IsRequired = true)]
		public GameRank OwnGameRank;

		[ProtoMember(4, IsRequired = true)]
		public WealthRank OwnWealthRank;

	}

	[Message(HotfixOpcode.C2G_DailySign)]
	[ProtoContract]
	public partial class C2G_DailySign: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

	}

	[Message(HotfixOpcode.G2C_DailySign)]
	[ProtoContract]
	public partial class G2C_DailySign: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Reward;

		[ProtoMember(2, IsRequired = true)]
		public string TomorrowReward;

	}

	[Message(HotfixOpcode.C2G_DailySignState)]
	[ProtoContract]
	public partial class C2G_DailySignState: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

	}

	[Message(HotfixOpcode.G2C_DailySignState)]
	[ProtoContract]
	public partial class G2C_DailySignState: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public bool TodayIsSign;

		[ProtoMember(2, IsRequired = true)]
		public string TodayReward;

		[ProtoMember(3, IsRequired = true)]
		public string TomorrowReward;

	}

	[Message(HotfixOpcode.C2G_RealName)]
	[ProtoContract]
	public partial class C2G_RealName: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public string Name;

		[ProtoMember(3, IsRequired = true)]
		public string IDNumber;

	}

	[Message(HotfixOpcode.G2C_RealName)]
	[ProtoContract]
	public partial class G2C_RealName: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

	[Message(HotfixOpcode.C2G_BindPhone)]
	[ProtoContract]
	public partial class C2G_BindPhone: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public string Phone;

		[ProtoMember(3, IsRequired = true)]
		public string Code;

	}

	[Message(HotfixOpcode.G2C_BindPhone)]
	[ProtoContract]
	public partial class G2C_BindPhone: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

	[Message(HotfixOpcode.C2G_SendSms)]
	[ProtoContract]
	public partial class C2G_SendSms: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public string Phone;

	}

	[Message(HotfixOpcode.G2C_SendSms)]
	[ProtoContract]
	public partial class G2C_SendSms: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

	[Message(HotfixOpcode.C2G_ChangeName)]
	[ProtoContract]
	public partial class C2G_ChangeName: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public string Name;

	}

	[Message(HotfixOpcode.G2C_ChangeName)]
	[ProtoContract]
	public partial class G2C_ChangeName: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

	[Message(HotfixOpcode.C2G_UseHuaFei)]
	[ProtoContract]
	public partial class C2G_UseHuaFei: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public int HuaFei;

		[ProtoMember(3, IsRequired = true)]
		public string Phone;

	}

	[Message(HotfixOpcode.G2C_UseHuaFei)]
	[ProtoContract]
	public partial class G2C_UseHuaFei: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

	[Message(HotfixOpcode.C2G_UseHuaFeiState)]
	[ProtoContract]
	public partial class C2G_UseHuaFeiState: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

	}

	[Message(HotfixOpcode.G2C_UseHuaFeiState)]
	[ProtoContract]
	public partial class G2C_UseHuaFeiState: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public int HuaFei_1_RestCount;

		[ProtoMember(2, IsRequired = true)]
		public int HuaFei_5_RestCount;

		[ProtoMember(3, IsRequired = true)]
		public int HuaFei_10_RestCount;

		[ProtoMember(4, IsRequired = true)]
		public int HuaFei_20_RestCount;

	}

	[Message(HotfixOpcode.C2G_SetPlayerSound)]
	[ProtoContract]
	public partial class C2G_SetPlayerSound: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public int PlayerSound;

	}

	[Message(HotfixOpcode.G2C_SetPlayerSound)]
	[ProtoContract]
	public partial class G2C_SetPlayerSound: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

	[Message(HotfixOpcode.C2G_UseLaBa)]
	[ProtoContract]
	public partial class C2G_UseLaBa: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

		[ProtoMember(2, IsRequired = true)]
		public string Content;

	}

	[Message(HotfixOpcode.G2C_UseLaBa)]
	[ProtoContract]
	public partial class G2C_UseLaBa: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

	[Message(HotfixOpcode.Actor_LaBa)]
	[ProtoContract]
	public partial class Actor_LaBa: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string LaBaContent;

	}

	[Message(HotfixOpcode.Actor_OldUser)]
	[ProtoContract]
	public partial class Actor_OldUser: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string OldAccount;

	}

	[Message(HotfixOpcode.Actor_Chat)]
	[ProtoContract]
	public partial class Actor_Chat: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public int ChatType;

		[ProtoMember(2, IsRequired = true)]
		public string Value;

		[ProtoMember(3, IsRequired = true)]
		public long UId;

	}

	[Message(HotfixOpcode.C2G_UseZhuanPan)]
	[ProtoContract]
	public partial class C2G_UseZhuanPan: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

	}

	[Message(HotfixOpcode.G2C_UseZhuanPan)]
	[ProtoContract]
	public partial class G2C_UseZhuanPan: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string reward;

		[ProtoMember(2, IsRequired = true)]
		public int itemId;

	}

	[Message(HotfixOpcode.C2G_GetZhuanPanState)]
	[ProtoContract]
	public partial class C2G_GetZhuanPanState: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

	}

	[Message(HotfixOpcode.G2C_GetZhuanPanState)]
	[ProtoContract]
	public partial class G2C_GetZhuanPanState: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public int ZhuanPanCount;

		[ProtoMember(2, IsRequired = true)]
		public int LuckyValue;

	}

	[Message(HotfixOpcode.C2G_Share)]
	[ProtoContract]
	public partial class C2G_Share: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Uid;

	}

	[Message(HotfixOpcode.G2C_Share)]
	[ProtoContract]
	public partial class G2C_Share: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

	[Message(HotfixOpcode.C2G_HeartBeat)]
	[ProtoContract]
	public partial class C2G_HeartBeat: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

	}

	[Message(HotfixOpcode.G2C_HeartBeat)]
	[ProtoContract]
	public partial class G2C_HeartBeat: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

//端午活动
	[Message(HotfixOpcode.TreasureInfo)]
	[ProtoContract]
	public partial class TreasureInfo: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public int TreasureId;

		[ProtoMember(2, IsRequired = true)]
		public string Reward;

		[ProtoMember(3, IsRequired = true)]
		public int Price;

		[ProtoMember(4, IsRequired = true)]
		public int LimitCount;

		[ProtoMember(5, IsRequired = true)]
		public string Name;

	}

	[Message(HotfixOpcode.C2G_DuanwuTreasure)]
	[ProtoContract]
	public partial class C2G_DuanwuTreasure: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UId;

	}

	[Message(HotfixOpcode.G2C_DuanwuTreasure)]
	[ProtoContract]
	public partial class G2C_DuanwuTreasure: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, TypeName = "ETHotfix.TreasureInfo")]
		public List<TreasureInfo> TreasureInfoList = new List<TreasureInfo>();

	}

	[Message(HotfixOpcode.DuanwuActivity)]
	[ProtoContract]
	public partial class DuanwuActivity: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public string Desc;

		[ProtoMember(2, IsRequired = true)]
		public int Reward;

		[ProtoMember(3, IsRequired = true)]
		public int TaskId;

		[ProtoMember(4, IsRequired = true)]
		public int CurProgress;

		[ProtoMember(5, IsRequired = true)]
		public int Target;

		[ProtoMember(6, IsRequired = true)]
		public bool IsComplete;

		[ProtoMember(7, IsRequired = true)]
		public bool IsGet;

	}

	[Message(HotfixOpcode.C2G_DuanwuActivity)]
	[ProtoContract]
	public partial class C2G_DuanwuActivity: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UId;

	}

	[Message(HotfixOpcode.G2C_DuanwuActivity)]
	[ProtoContract]
	public partial class G2C_DuanwuActivity: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, TypeName = "ETHotfix.DuanwuActivity")]
		public List<DuanwuActivity> DuanwuActivityList = new List<DuanwuActivity>();

	}

	[Message(HotfixOpcode.C2G_RefreshDuanwuActivity)]
	[ProtoContract]
	public partial class C2G_RefreshDuanwuActivity: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UId;

		[ProtoMember(2, IsRequired = true)]
		public long GoldCost;

		[ProtoMember(3, IsRequired = true)]
		public string ActivityType;

	}

	[Message(HotfixOpcode.G2C_RefreshDuanwuActivity)]
	[ProtoContract]
	public partial class G2C_RefreshDuanwuActivity: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, TypeName = "ETHotfix.DuanwuActivity")]
		public List<DuanwuActivity> DuanwuActivityList = new List<DuanwuActivity>();

		[ProtoMember(2, IsRequired = true)]
		public DuanwuData DuanwuData;

		[ProtoMember(3, IsRequired = true)]
		public long GoldNum;

		[ProtoMember(4, IsRequired = true)]
		public bool isOpen;

	}

	[Message(HotfixOpcode.C2G_BuyDuanwuTreasure)]
	[ProtoContract]
	public partial class C2G_BuyDuanwuTreasure: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UId;

		[ProtoMember(2, IsRequired = true)]
		public int TreasureId;

		[ProtoMember(3, IsRequired = true)]
		public string Reward;

		[ProtoMember(4, IsRequired = true)]
		public int Price;

		[ProtoMember(5, IsRequired = true)]
		public int LimitCount;

	}

	[Message(HotfixOpcode.G2C_BuyDuanwuTreasure)]
	[ProtoContract]
	public partial class G2C_BuyDuanwuTreasure: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public DuanwuTreasureLogInfo Info;

		[ProtoMember(2, IsRequired = true)]
		public int ZongZiCount;

	}

	[Message(HotfixOpcode.DuanwuData)]
	[ProtoContract]
	public partial class DuanwuData: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public int ZongziCount;

		[ProtoMember(2, IsRequired = true)]
		public string ActivityType;

		[ProtoMember(3, IsRequired = true)]
		public int RefreshCount;

		[ProtoMember(4, IsRequired = true)]
		public string StartTime;

		[ProtoMember(5, IsRequired = true)]
		public string EndTime;

		[ProtoMember(6, IsRequired = true)]
		public int CompleteCount;

	}

	[Message(HotfixOpcode.C2G_DuanwuDataBase)]
	[ProtoContract]
	public partial class C2G_DuanwuDataBase: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UId;

		[ProtoMember(2, IsRequired = true)]
		public string ActivityType;

		[ProtoMember(3, IsRequired = true)]
		public int ZongZiCount;

		[ProtoMember(4, IsRequired = true)]
		public int Type;

	}

	[Message(HotfixOpcode.G2C_DuanwuDataBase)]
	[ProtoContract]
	public partial class G2C_DuanwuDataBase: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public DuanwuData DuanwuData;

	}

	[Message(HotfixOpcode.C2G_UpdateServer)]
	[ProtoContract]
	public partial class C2G_UpdateServer: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

	}

	[Message(HotfixOpcode.G2C_UpdateServer)]
	[ProtoContract]
	public partial class G2C_UpdateServer: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

	[Message(HotfixOpcode.C2G_GetDuanwuReward)]
	[ProtoContract]
	public partial class C2G_GetDuanwuReward: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UId;

		[ProtoMember(2, IsRequired = true)]
		public int TaskId;

		[ProtoMember(3, IsRequired = true)]
		public int Reward;

	}

	[Message(HotfixOpcode.G2C_GetDuanwuReward)]
	[ProtoContract]
	public partial class G2C_GetDuanwuReward: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public int ZongziCount;

		[ProtoMember(2, IsRequired = true)]
		public bool IsGet;

	}

	[Message(HotfixOpcode.DuanwuTreasureLogInfo)]
	[ProtoContract]
	public partial class DuanwuTreasureLogInfo: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public int TreasureId;

		[ProtoMember(2, IsRequired = true)]
		public int buyCount;

	}

	[Message(HotfixOpcode.C2G_GetDuanwuTreasureInfo)]
	[ProtoContract]
	public partial class C2G_GetDuanwuTreasureInfo: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UId;

	}

	[Message(HotfixOpcode.G2C_GetDuanwuTreasureInfo)]
	[ProtoContract]
	public partial class G2C_GetDuanwuTreasureInfo: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, TypeName = "ETHotfix.DuanwuTreasureLogInfo")]
		public List<DuanwuTreasureLogInfo> Treasures = new List<DuanwuTreasureLogInfo>();

	}

}
