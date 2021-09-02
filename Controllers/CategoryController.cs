using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Category>>> Get([FromServices] DataContext context)
        {
            // Recuperando todas categorias do banco
            var categories = await context.Categories.AsNoTracking().ToListAsync();

            if (categories.Count == 0)
                return NotFound(new { message = "Nenhuma categoria cadastrada" });

            return Ok(categories);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Category>> GetById(
            int id,
            [FromServices] DataContext context)
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound(new { message = "Categoria não encontrada" });

            return Ok(category);
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Category>> Post(
            [FromBody] Category categoryModel,
            [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Adicionar categoria
                context.Categories.Add(categoryModel);

                // Persistir no banco
                await context.SaveChangesAsync();

                return Ok(categoryModel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Não foi possível criar a categoria. Retorno: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult<Category>> Put(
            int id,
            [FromBody] Category categoryModel,
            [FromServices] DataContext context)
        {
            if (categoryModel.Id != id)
                return NotFound(new { message = "Categoria não encontrada" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Atualizar registro
                context.Entry<Category>(categoryModel).State = EntityState.Modified;

                // Persistir no banco
                await context.SaveChangesAsync();

                return Ok(categoryModel);
            }
            catch (DbUpdateConcurrencyException currencyException)
            // Mais catchs no try/catch (Sempre do mais especifico para o mais genérico)
            {
                return BadRequest(new { message = $"Ocorreu um erro de concorrência. Retorno: {currencyException.Message}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Não foi possível atualizar a categoria. Retorno: {ex.Message}" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult<Category>> Delete(
            int id,
            [FromServices] DataContext context)
        {
            // Recupera categoria do banco
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound(new { message = "Categoria não encontrada" });

            try
            {
                // Removendo categoria encontrada
                context.Categories.Remove(category);

                // Persistindo alteração no banco
                await context.SaveChangesAsync();
                return Ok(new { message = "Categoria removida com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Não foi possível excluir a categoria. Retorno: {ex.Message}" });
            }
        }
    }
}