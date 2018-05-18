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

		[ProtoMember(3)]
		public List<ShopInfo> ShopInfoList = new List<ShopInfo>();

		[ProtoMember(4)]
		public List<TaskInfo> TaskInfoList = new List<TaskInfo>();

		[ProtoMember(5)]
		public List<Bag> BagList = new List<Bag>();

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
		public int Cost;

	}

	[Message(HotfixOpcode.Actor_RefreshRank)]
	[ProtoContract]
	public partial class Actor_RefreshRank: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1)]
		public List<PlayerInfo> playerInfo = new List<PlayerInfo>();

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
		public bool Result;

	}

	[Message(HotfixOpcode.TaskProgress)]
	[ProtoContract]
	public partial class TaskProgress: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public int TaskId;

		[ProtoMember(2, IsRequired = true)]
		public int Progress;

		[ProtoMember(3, IsRequired = true)]
		public int Target;

		[ProtoMember(4, IsRequired = true)]
		public bool IsComplete;

		[ProtoMember(5, IsRequired = true)]
		public bool IsGet;

	}

	[Message(HotfixOpcode.C2G_UpdateTaskProgress)]
	[ProtoContract]
	public partial class C2G_UpdateTaskProgress: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UId;

		[ProtoMember(2, IsRequired = true)]
		public TaskInfo TaskPrg;

	}

	[Message(HotfixOpcode.G2C_UpdateTaskProgress)]
	[ProtoContract]
	public partial class G2C_UpdateTaskProgress: IResponse
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

		[ProtoMember(1)]
		public List<TaskInfo> TaskProgressList = new List<TaskInfo>();

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

	[Message(HotfixOpcode.C2G_EnterRoom)]
	[ProtoContract]
	public partial class C2G_EnterRoom: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

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

		[ProtoMember(1)]
		public List<GamerInfo> Gamers = new List<GamerInfo>();

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

	[Message(HotfixOpcode.Actor_StartGame)]
	[ProtoContract]
	public partial class Actor_StartGame: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1)]
		public List<MahjongInfo> Mahjongs = new List<MahjongInfo>();

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
		public long EId;

	}

	[Message(HotfixOpcode.C2G_GetItem)]
	[ProtoContract]
	public partial class C2G_GetItem: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long UId;

		[ProtoMember(2)]
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

		[ProtoMember(1)]
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

		[ProtoMember(1)]
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

}
