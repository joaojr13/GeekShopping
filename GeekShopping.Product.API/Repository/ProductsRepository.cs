using AutoMapper;
using GeekShopping.Product.API.Data.ValueObjects;
using GeekShopping.Product.API.Models;
using GeekShopping.Product.API.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.Product.API.Repository
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly MySQLContext _context;
        private IMapper _mapper;

        public ProductsRepository(MySQLContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ProductsVO>> FindAll()
        {
            List<Products> products = await _context.Products.ToListAsync();
            return _mapper.Map<List<ProductsVO>>(products);
        }

        public async Task<ProductsVO> FindById(long id)
        {
            Products product = await _context.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<ProductsVO>(product);
        }
        public async Task<ProductsVO> Create([FromBody] ProductsVO vo)
        {
            Products product = _mapper.Map<Products>(vo);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProductsVO>(product);
        }
        public async Task<ProductsVO> Update([FromBody] ProductsVO vo)
        {
            Products product = _mapper.Map<Products>(vo);
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProductsVO>(product);
        }

        public async Task<bool> Delete(long id)
        {
            try
            {
                Products product = await _context.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
                if (product == null) return false;

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
