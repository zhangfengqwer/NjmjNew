using System.Collections.Generic;
using ETHotfix;

namespace ETModel
{
    [ObjectSystem]
    public class OperationComponentSystem : AwakeSystem<OperationComponent>
    {
        public override void Awake(OperationComponent self)
        {
            self.Awake();
        }
    }

    public class OperationComponent : Component
    {

        public void Awake()
        {
        }

        public override void Dispose()
        {
            if(this.IsDisposed)
            {
                return;
            }

            base.Dispose();
        }

       
    }
}
