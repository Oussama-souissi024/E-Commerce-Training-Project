using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;
using E_Commerce.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services
{
    public class CouponService : ICoupon
    {
        private readonly IRepository<Coupon> _repository;
        private readonly ApplicationbDbContext _context;
        public CouponService(IRepository<Coupon> repository, ApplicationbDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<Coupon> AddAsync(Coupon coupon)
        {
            var addedCoupon = await _repository.AddAsync(coupon);
            _repository.SaveChanges();
            return addedCoupon;
        }

        public async Task<Coupon> ReadByIdAsync(int id)
        {
            return await _repository.ReadById(id);
        }

        public Task<Coupon> ReadByCouponCodeAsync(string couponCode)
        {
            var couponFromDb =  _context.Coupons.FirstOrDefaultAsync(c => c.CouponCode.ToLower() == couponCode.ToLower());
            return  couponFromDb;
        }

        public async Task<IEnumerable<Coupon>> ReadAllAsync()
        {
            return await _repository.ReadAll();
        }

        public void Update(Coupon coupon)
        {
            _repository.Update(coupon);
            _repository.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
            _repository.SaveChanges();
        }
    }
}
