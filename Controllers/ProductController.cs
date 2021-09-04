using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    // https:localhost:5001/products
    [Route("v1/products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
        {
            // Recupera todos os produtos já vinculados ao Category
            var products = await context
                .Products
                .Include(x => x.Category) // Vincula o Category no Produto com todas suas informações
                .AsNoTracking()
                .ToListAsync();

            if (products.Count == 0)
                return NotFound(new { message = "Nenhum produto cadastrado" });

            return Ok(products);
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(
            int id,
            [FromServices] DataContext context)
        {
            var product = await context
                .Products
                .Include(x => x.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
                return NotFound(new { message = "Produto não encontrado" });

            return Ok(product);
        }

        [HttpGet]
        [Route("/categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory(
            int categoryId,
            [FromServices] DataContext context)
        {
            var products = await context
                .Products
                .Include(x => x.Category)
                .AsNoTracking()
                .Where(x => x.Category.Id == categoryId)
                .ToListAsync();

            if (products.Count == 0)
                return NotFound(new { message = $"Não existe produto(s) para categoria {categoryId}" });

            return Ok(products);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Post(
            [FromBody] Product productModel,
            [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Products.Add(productModel);
                await context.SaveChangesAsync();
                return Ok(productModel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Não foi possível criar a produto. Retorno: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Product>> Put(
            int id,
            [FromBody] Product productModel,
            [FromServices] DataContext context)
        {
            if (id != productModel.Id)
                return NotFound(new { message = "Produto não encontrado" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Product>(productModel).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(productModel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Não foi possível atualizar produto. Retorno: {ex.Message}" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Product>> Delete(
            int id,
            [FromServices] DataContext context)
        {
            var product = await context
                .Products
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
                return NotFound(new { message = "Produto não encontrado" });

            try
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return Ok(new { message = "Produto excluído com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Não foi possível excluir produto. Retorno: {ex.Message}" });
            }
        }
    }
}