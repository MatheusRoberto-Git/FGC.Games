using FGC.Games.Application.DTOs;
using FGC.Games.Application.UseCases;
using FGC.Games.Domain.Enums;
using FGC.Games.Presentation.Models.Requests;
using FGC.Games.Presentation.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FGC.Games.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly CreateGameUseCase _createGameUseCase;
        private readonly GetGameByIdUseCase _getGameByIdUseCase;
        private readonly GetAllGamesUseCase _getAllGamesUseCase;
        private readonly SearchGamesUseCase _searchGamesUseCase;
        private readonly UpdateGamePriceUseCase _updateGamePriceUseCase;
        private readonly DeactivateGameUseCase _deactivateGameUseCase;
        private readonly ActivateGameUseCase _activateGameUseCase;

        public GamesController(CreateGameUseCase createGameUseCase, GetGameByIdUseCase getGameByIdUseCase, GetAllGamesUseCase getAllGamesUseCase,
            SearchGamesUseCase searchGamesUseCase, UpdateGamePriceUseCase updateGamePriceUseCase, DeactivateGameUseCase deactivateGameUseCase, ActivateGameUseCase activateGameUseCase)
        {
            _createGameUseCase = createGameUseCase;
            _getGameByIdUseCase = getGameByIdUseCase;
            _getAllGamesUseCase = getAllGamesUseCase;
            _searchGamesUseCase = searchGamesUseCase;
            _updateGamePriceUseCase = updateGamePriceUseCase;
            _deactivateGameUseCase = deactivateGameUseCase;
            _activateGameUseCase = activateGameUseCase;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GameResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<GameResponse>>>> GetAll([FromQuery] bool onlyActive = true)
        {
            try
            {
                var result = await _getAllGamesUseCase.ExecuteAsync(onlyActive);

                var response = result.Select(g => new GameResponse
                {
                    Id = g.Id,
                    Title = g.Title,
                    Description = g.Description,
                    Price = g.Price,
                    Category = g.Category,
                    Developer = g.Developer,
                    Publisher = g.Publisher,
                    ReleaseDate = g.ReleaseDate,
                    CreatedAt = g.CreatedAt,
                    UpdatedAt = g.UpdatedAt,
                    IsActive = g.IsActive,
                    Rating = g.Rating,
                    TotalSales = g.TotalSales
                });

                return Ok(ApiResponse<IEnumerable<GameResponse>>.SuccessResult(response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<GameResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<GameResponse>>> GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorResult("ID inválido"));

                var result = await _getGameByIdUseCase.ExecuteAsync(id);

                var response = new GameResponse
                {
                    Id = result.Id,
                    Title = result.Title,
                    Description = result.Description,
                    Price = result.Price,
                    Category = result.Category,
                    Developer = result.Developer,
                    Publisher = result.Publisher,
                    ReleaseDate = result.ReleaseDate,
                    CreatedAt = result.CreatedAt,
                    UpdatedAt = result.UpdatedAt,
                    IsActive = result.IsActive,
                    Rating = result.Rating,
                    TotalSales = result.TotalSales
                };

                return Ok(ApiResponse<GameResponse>.SuccessResult(response));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GameResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<GameResponse>>>> Search([FromQuery] SearchGamesRequest request)
        {
            try
            {
                var dto = new SearchGamesDTO
                {
                    SearchTerm = request.SearchTerm ?? string.Empty,
                    Category = request.Category.HasValue ? (GameCategory)request.Category.Value : null,
                    MinPrice = request.MinPrice,
                    MaxPrice = request.MaxPrice,
                    OnlyActive = true
                };

                var result = await _searchGamesUseCase.ExecuteAsync(dto);

                var response = result.Select(g => new GameResponse
                {
                    Id = g.Id,
                    Title = g.Title,
                    Description = g.Description,
                    Price = g.Price,
                    Category = g.Category,
                    Developer = g.Developer,
                    Publisher = g.Publisher,
                    ReleaseDate = g.ReleaseDate,
                    CreatedAt = g.CreatedAt,
                    UpdatedAt = g.UpdatedAt,
                    IsActive = g.IsActive,
                    Rating = g.Rating,
                    TotalSales = g.TotalSales
                });

                return Ok(ApiResponse<IEnumerable<GameResponse>>.SuccessResult(response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<GameResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<GameResponse>>> Create([FromBody] CreateGameRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(ApiResponse<object>.ErrorResult("Dados obrigatórios"));

                if (string.IsNullOrWhiteSpace(request.Title))
                    return BadRequest(ApiResponse<object>.ErrorResult("Título é obrigatório"));

                if (string.IsNullOrWhiteSpace(request.Description))
                    return BadRequest(ApiResponse<object>.ErrorResult("Descrição é obrigatória"));

                if (request.Price < 0)
                    return BadRequest(ApiResponse<object>.ErrorResult("Preço não pode ser negativo"));

                var dto = new CreateGameDTO
                {
                    Title = request.Title,
                    Description = request.Description,
                    Price = request.Price,
                    Category = (GameCategory)request.Category,
                    Developer = request.Developer,
                    Publisher = request.Publisher,
                    ReleaseDate = request.ReleaseDate
                };

                var result = await _createGameUseCase.ExecuteAsync(dto);

                var response = new GameResponse
                {
                    Id = result.Id,
                    Title = result.Title,
                    Description = result.Description,
                    Price = result.Price,
                    Category = result.Category,
                    Developer = result.Developer,
                    Publisher = result.Publisher,
                    ReleaseDate = result.ReleaseDate,
                    CreatedAt = result.CreatedAt,
                    UpdatedAt = result.UpdatedAt,
                    IsActive = result.IsActive,
                    Rating = result.Rating,
                    TotalSales = result.TotalSales
                };

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Id },
                    ApiResponse<GameResponse>.SuccessResult(response, "Jogo criado com sucesso")
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpPut("{id}/price")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<GameResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<GameResponse>>> UpdatePrice(Guid id, [FromBody] UpdateGamePriceRequest request)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorResult("ID inválido"));

                if (request == null)
                    return BadRequest(ApiResponse<object>.ErrorResult("Dados obrigatórios"));

                if (request.NewPrice < 0)
                    return BadRequest(ApiResponse<object>.ErrorResult("Preço não pode ser negativo"));

                var dto = new UpdateGamePriceDTO
                {
                    GameId = id,
                    NewPrice = request.NewPrice
                };

                var result = await _updateGamePriceUseCase.ExecuteAsync(dto);

                var response = new GameResponse
                {
                    Id = result.Id,
                    Title = result.Title,
                    Description = result.Description,
                    Price = result.Price,
                    Category = result.Category,
                    Developer = result.Developer,
                    Publisher = result.Publisher,
                    ReleaseDate = result.ReleaseDate,
                    CreatedAt = result.CreatedAt,
                    UpdatedAt = result.UpdatedAt,
                    IsActive = result.IsActive,
                    Rating = result.Rating,
                    TotalSales = result.TotalSales
                };

                return Ok(ApiResponse<GameResponse>.SuccessResult(response, "Preço atualizado com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpPut("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<GameResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<GameResponse>>> Deactivate(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorResult("ID inválido"));

                var result = await _deactivateGameUseCase.ExecuteAsync(id);

                var response = new GameResponse
                {
                    Id = result.Id,
                    Title = result.Title,
                    Description = result.Description,
                    Price = result.Price,
                    Category = result.Category,
                    Developer = result.Developer,
                    Publisher = result.Publisher,
                    ReleaseDate = result.ReleaseDate,
                    CreatedAt = result.CreatedAt,
                    UpdatedAt = result.UpdatedAt,
                    IsActive = result.IsActive,
                    Rating = result.Rating,
                    TotalSales = result.TotalSales
                };

                return Ok(ApiResponse<GameResponse>.SuccessResult(response, "Jogo desativado com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpPut("{id}/activate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<GameResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<GameResponse>>> Activate(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorResult("ID inválido"));

                var result = await _activateGameUseCase.ExecuteAsync(id);

                var response = new GameResponse
                {
                    Id = result.Id,
                    Title = result.Title,
                    Description = result.Description,
                    Price = result.Price,
                    Category = result.Category,
                    Developer = result.Developer,
                    Publisher = result.Publisher,
                    ReleaseDate = result.ReleaseDate,
                    CreatedAt = result.CreatedAt,
                    UpdatedAt = result.UpdatedAt,
                    IsActive = result.IsActive,
                    Rating = result.Rating,
                    TotalSales = result.TotalSales
                };

                return Ok(ApiResponse<GameResponse>.SuccessResult(response, "Jogo ativado com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Erro interno do servidor - {ex.Message}"));
            }
        }
    }
}