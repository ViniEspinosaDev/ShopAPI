using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("v1/users")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
        {
            var users = await context.Users.AsNoTracking().ToListAsync();

            if (users.Count == 0)
                return NotFound(new { message = "Nenhum usuário cadastrado" });

            return Ok(users);
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromBody] User userModel,
            [FromServices] DataContext context)
        {
            var user = await context
                .Users
                .AsNoTracking()
                .Where(x => x.Username == userModel.Username && x.Password == userModel.Password)
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            var token = TokenService.GenerateToken(user);
            user.Password = "";
            return new
            {
                user = user,
                token = token
            };
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Post(
            [FromBody] User userModel,
            [FromServices] DataContext context)
        {
            userModel.Role = "employee";

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Users.Add(userModel);
                await context.SaveChangesAsync();
                userModel.Password = "****";
                return Ok(userModel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Não foi possível criar usuário. Retorno: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put(
            int id,
            [FromBody] User userModel,
            [FromServices] DataContext context)
        {
            if (id != userModel.Id)
                return NotFound(new { message = "Usuário não encontrado" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<User>(userModel).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(userModel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Não foi possível atualizar usuário. Retorno: {ex.Message}" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Delete(
            int id,
            [FromServices] DataContext context)
        {
            var user = await context
                .Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return NotFound(new { message = "Usuário não encontrado" });

            try
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return Ok(new { message = "Usuário excluído com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Não foi possível excluir Usuário. Retorno: {ex.Message}" });
            }
        }
    }
}