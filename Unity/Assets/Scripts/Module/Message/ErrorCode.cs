namespace ETModel
{
	public static class ErrorCode
	{
	    public const int ERR_Success = 0;
		public const int ERR_NotFoundActor = 2;

		public const int ERR_AccountOrPasswordError = 102;
		public const int ERR_SessionActorError = 103;
		public const int ERR_NotFoundUnit = 104;
		public const int ERR_ConnectGateKeyError = 105;
        public const int ERR_TokenError = 106;
        public const int ERR_AccountNoExist = 107;
        public const int ERR_ParamError = 108;
        public const int ERR_PhoneCodeError = 109;

        public const int AccountExist = 110;
        public const int TodayHasSign = 111;

        public const int ERR_Exception = 1000;

		public const int ERR_RpcFail = 2001;
		public const int ERR_SocketDisconnected = 2002;
		public const int ERR_ReloadFail = 2003;
		public const int ERR_ActorLocationNotFound = 2004;
    }
}