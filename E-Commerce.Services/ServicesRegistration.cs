using E_Commerce.Core.Interfaces;
using E_Commerce.Services.MappinProfile;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services
{
    public static class ServicesRegistration
    {
        public static void AddServiceRegistraion(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IProduct, ProductServices>();
            builder.Services.AddTransient<ICategory, CategoryServices>();
            builder.Services.AddTransient<ICart, CartService>();
            builder.Services.AddTransient<ICoupon, CouponService>();
            builder.Services.AddTransient<IFileHelper, FileHelper>();
            builder.Services.AddTransient<IOrder, OrderServices>();
            builder.Services.AddTransient<IStripeSession, StripeSessionService>();

            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddAutoMapper(typeof(CouponMapping));
            builder.Services.AddAutoMapper(typeof(ProductMapping));
            builder.Services.AddAutoMapper(typeof(CategoryMapping));
            builder.Services.AddAutoMapper(typeof(CartMapping));
            builder.Services.AddAutoMapper(typeof(OderMapping));

        }
    }
}
