using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.Dtos;
using SdtdServerKit.Data.IRepositories;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Mapster;

namespace SdtdServerKit.WebApi.Controllers
{
    /// <summary>
    /// Cd Key
    /// </summary>
    [Authorize]
    [RoutePrefix("api/CdKeys")]
    public class CdKeysController : ApiController
    {
        private readonly ICdKeyRepository _cdKeyRepository;

        public CdKeysController(ICdKeyRepository cdKeyRepository)
        {
            _cdKeyRepository = cdKeyRepository;
        }

        /// <summary>
        /// Get all Cd Keys.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IEnumerable<CdKeyDto>> GetCdKeys()
        {
            return (await _cdKeyRepository.GetAllAsync()).Adapt<IEnumerable<CdKeyDto>>();
        }

        /// <summary>
        /// Get a Cd Key by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(CdKeyDto))]
        public async Task<IHttpActionResult> GetCdKey([FromUri] int id)
        {
            var entity = await _cdKeyRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity.Adapt<CdKey>());
        }

        /// <summary>
        /// Creates a new Cd Key.
        /// </summary>
        /// <param name="dto">The Cd Key dto to create.</param>
        /// <returns>An IHttpActionResult indicating the result of the operation.</returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> CreateCdKey([FromBody] CdKeyCreateDto dto)
        {
            var entity = dto.Adapt<CdKey>();
            entity.CreatedAt = DateTime.Now;
            int id = await _cdKeyRepository.InsertAsync<int>(entity);
            var location = new Uri(Request.RequestUri, $"api/CdKey/{id}");

            return Created(location, entity);
        }

        /// <summary>
        /// Updates an existing Cd Key.
        /// </summary>
        /// <param name="id">The ID of the Cd Key to update.</param>
        /// <param name="dto">The updated Cd Key dto.</param>
        /// <returns>An IHttpActionResult indicating the result of the operation.</returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> UpdateCdKey([FromUri] int id, [FromBody] CdKeyUpdateDto dto)
        {
            var entity = await _cdKeyRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            dto.Adapt(entity);
            await _cdKeyRepository.UpdateAsync(entity);

            return Ok();
        }

        /// <summary>
        /// Deletes a Cd Key by ID.
        /// </summary>
        /// <param name="id">The ID of the Cd Key to delete.</param>
        /// <returns>An IHttpActionResult indicating the result of the operation.</returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteCdKey([FromUri] int id)
        {
            int count = await _cdKeyRepository.DeleteByIdAsync(id);
            if (count == 0)
            {
                return NotFound();
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Deletes multiple Cd Key by their IDs.
        /// </summary>
        /// <param name="ids">The IDs of the Cd Key to delete.</param>
        /// <returns>An IHttpActionResult indicating the result of the operation.</returns>
        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> DeleteCdKeys([FromUri, Required, MinLength(1)] int[] ids)
        {
            int count = await _cdKeyRepository.DeleteByIdsAsync(ids, true);
            if (count == 0)
            {
                return NotFound();
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
