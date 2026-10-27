using Domain.IRepository;
using Application.Services.Auth;
using Infrastructure.Repository;
using Application.Services.Otp;
using Application.Services.Chats;
using Application.Services.Messages;
using Application.Services.ChatMembers;
using Application.Services.UserManagement;
using Application.Interfaces;
using Application.Services.Cache;
using Api.SignalR;
using Application.Services.BlockedUser;
using Application.Services.Status;
using Application.Services.File;
using Application.Services.Status.Viewer;
using Application.Services.Contact;
using Api.Services;
using Application.Services.Contact.ContactName;

namespace Api.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository , UserRepository>();
            services.AddScoped<IRefreshRepository, RefreshRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<IBlockedUserRepository,BlockedUserRepository>();
            services.AddScoped<IStatusRepository, StatusRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IViewStatusRepository, ViewStatusRepository>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IMemberService, MemberService>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddScoped<IBlockedUserService, BlockedUserService>();
            services.AddScoped<IStatusService, StatusService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IContactNameService, ContactNameService>();

            services.AddScoped<IChatNotificationService, ChatNotificationService>();
            services.AddScoped<IMessageNotificationService, MessageNotificationService>();
            services.AddScoped<IMemberNotificationService, MemberNotificationService>();
            services.AddScoped<IStatusNotificationService, StatusNotificationService>();
            services.AddSingleton<IUserConnectionTracker, UserConnectionTracker>();
            return services;
        }
    }
}
