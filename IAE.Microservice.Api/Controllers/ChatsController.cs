using System.Threading.Tasks;
using IAE.Microservice.Application.Features.Chats;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IAE.Microservice.Api.Controllers
{
    [ApiController]
    [Route("Chat")]
    public class ChatsController : BaseController
    {
        /// <summary>
        /// Создать чат
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> ChatCreate([FromBody] Create.Command command)
        {
            return StatusCode(405, "В разработке");
        }
        
        /// <summary>
        /// Получить список чатов для пользователя
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> List(long id)
        {
            return StatusCode(405, "В разработке");
        }

        /// <summary>
        /// Получить список сообщений из чата
        /// </summary>
        /// <param name="id">id чата</param>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMessages(long id, [FromQuery] ChatHistory.Query query)
        {
            query.Id = id;
            return StatusCode(405, "В разработке");
        }
        
        /// <summary>
        /// Отправить сообщение в чат
        /// </summary>
        /// <param name="id">id чата</param>
        [HttpPost("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SendMessage(long id, [FromBody] Send.Command command)
        {
            return StatusCode(405, "В разработке");
        }
        
        /// <summary>
        /// Удалить чат
        /// </summary>
        /// <param name="id">id чата</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteChat(long id)
        {
            return StatusCode(405, "В разработке");
        }
        
        /// <summary>
        /// Удалить сообщение
        /// </summary>
        /// <param name="id">id сообщения</param>
        [HttpDelete("message/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteMessage(long id, [FromBody] DeleteMessage.Command command)
        {
            command.MessageId = id;
            return StatusCode(405, "В разработке");
        }
        
        /// <summary>
        /// Изменить сообщение
        /// </summary>
        /// <param name="id">id сообщения</param>
        [HttpPut("message/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> EditMessage(long id, [FromBody] Edit.Command command)
        {
            command.MessageId = id;
            return StatusCode(405, "В разработке");
        }
        
        /// <summary>
        /// Покинуть чат
        /// </summary>
        /// <param name="id">id чата</param>
        [HttpGet("{id}/leave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Leave(long id)
        {
            return StatusCode(405, "В разработке");
        }
        
        /// <summary>
        /// Ожидать сообщение в чате
        /// </summary>
        /// <param name="id">id чата</param>
        [HttpGet("message/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> WaitMessage(long id)
        {
            return StatusCode(405, "В разработке");
        }
        
        /// <summary>
        /// Войти в существующий чат
        /// </summary>
        /// <param name="id">id чата</param>
        [HttpGet("{id}/enter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ChatEnter(long id)
        {
            return StatusCode(405, "В разработке");
        }
    }
}