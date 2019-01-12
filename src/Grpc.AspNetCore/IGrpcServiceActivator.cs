namespace Grpc.AspNetCore
{
    interface IGrpcServiceActivator<TGrpcService> where TGrpcService : class
    {
        TGrpcService Create();
        void Release(TGrpcService service);
    }
}
