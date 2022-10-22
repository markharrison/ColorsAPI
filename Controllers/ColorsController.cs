using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ColorsAPI.Services;
using ColorsAPI.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace ColorsAPI.Controllers
{
    [Route("colors")]
    [ApiController]
    public class ColorsController : ControllerBase
    {

        private readonly ColorsService _ColorsService;
        public ColorsController(ColorsService ColorsService)
        {
            _ColorsService = ColorsService;
        }


        [HttpGet]
        [SwaggerOperation(
            Summary = "Get colors",
            Description = "Returns all colors.",
            OperationId = "GetColors",
            Tags = new[] { "Colors" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Success - returns list of colors", typeof(List<ColorsItem>))]
        public async Task<IActionResult> GetAllAsync()
        {
            List<ColorsItem> _ColorsList;
            _ColorsList = await _ColorsService.GetAll();
            return Ok(_ColorsList);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Update / create colors",
            Description = "Updates colors - creates color if it doesn't exist",
            OperationId = "UpdateColors",
            Tags = new[] { "Colors" }
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Success - colors updated/created", typeof(ColorsItem))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity", typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateAsync(
            [FromBody, SwaggerRequestBody("Colors to update", Required = true)] List<ColorsItem> colorsItems)
        {
            List<ColorsItem> _ColorsInserted = new() { };

            foreach (ColorsItem colorsItem in colorsItems) // Loop through List with foreach
            {
                if (colorsItem.Name == null || colorsItem.Name.Length == 0)
                {
                    return UnprocessableEntity(new ProblemDetails { Status = 422, Title = "Missing a Color Name" });
                }

                ColorsItem colorsItemReturn = await _ColorsService.UpdateById(0, colorsItem);
                _ColorsInserted.Add(colorsItemReturn);

            }

            List<ColorsItem> _ColorsList = await _ColorsService.GetAll();

            return Created("Colors/", _ColorsInserted);
        }

        [HttpDelete]
        [SwaggerOperation(
            Summary = "Delete colors",
            Description = "Deletes all colors.",
            OperationId = "DeletesColors",
            Tags = new[] { "Colors" }
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Success - all colors deleted", typeof(ColorsItem))]
        public async Task<IActionResult> DeleteAllAsync()
        {

            await _ColorsService.DeleteAll();

            return NoContent();
        }

        [HttpGet("{colorId}")]
        [SwaggerOperation(
            Summary = "Get color by id",
            Description = "Returns color specified by {colorId} (must be between 1 and 1000).",
            OperationId = "GetColorById",
            Tags = new[] { "Colors" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Success - returns color", typeof(ColorsItem))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity", typeof(ProblemDetails))]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute, SwaggerParameter("Id of Color to return", Required = true)] int colorId)
        {
            if (colorId < 1 || colorId > 1000)
            {
                return UnprocessableEntity(new ProblemDetails { Status = 422, Title = "{colorId} must be between 1 and 1000" });
            }

            ColorsItem _ColorsItem = await _ColorsService.GetById(colorId);
            if (_ColorsItem == null)
            {
                return NotFound(new ProblemDetails { Status = 404, Title = "Not Found - {colorId}: " + colorId });
            }

            return Ok(_ColorsItem);
        }

        [HttpPost("{colorId}")]
        [SwaggerOperation(
            Summary = "Update / create color by id",
            Description = "Updates color specified by {colorId} (must be between 1 and 1000);  use {colorId} = 0 to insert new color",
            OperationId = "UpdateColorById",
            Tags = new[] { "Colors" }
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Success - color created/updated", typeof(ColorsItem))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity", typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateByIdAsync(
                    [FromRoute, SwaggerParameter("Id of Color to update", Required = true)] int colorId,
                    [FromBody, SwaggerRequestBody("Colors to update", Required = true)] ColorsItem colorsItemUpdate)
        {
            if (colorId < 0 || colorId > 1000)
            {
                return UnprocessableEntity(new ProblemDetails { Status = 422, Title = "Unprocessable Entity - {id} must be between 0 and 1000" });
            }
            if (colorsItemUpdate.Name == null || colorsItemUpdate.Name.Length == 0)
            {
                return UnprocessableEntity(new ProblemDetails { Status = 422, Title = "Unprocessable Entity - Needs a Color Name" });
            }
            if (colorsItemUpdate.Id != colorId)
            {
                return UnprocessableEntity(new ProblemDetails { Status = 422, Title = "Unprocessable Entity - payload Id doesnt match {colorId}" });
            }

            ColorsItem colorsItemReturn = await _ColorsService.UpdateById(colorId, colorsItemUpdate);

            return Created("Colors/" + colorId, colorsItemReturn);

        }

        [HttpDelete("{colorId}")]
        [SwaggerOperation(
            Summary = "Delete color by id",
            Description = "Deletes color specified by {colorId} (must be between 1 and 1000).",
            OperationId = "DeleteColorById",
            Tags = new[] { "Colors" }
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Success - color deleted", typeof(ColorsItem))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity", typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteByIdAsync(
            [FromRoute, SwaggerParameter("Id of Color to delete", Required = true)] int colorId)
        {
            if (colorId < 1 || colorId > 1000)
            {
                return UnprocessableEntity(new ProblemDetails { Status = 422, Title = "Unprocessable Entity - {id} must be between 1 and 1000" });
            }

            await _ColorsService.DeleteById(colorId);

            return NoContent();
        }

        [Route("findbyname")]
        [HttpGet]
        [SwaggerOperation(
             Summary = "Get color by name",
             Description = "Returns color specified by {colorName} ",
             OperationId = "GetColorByName",
             Tags = new[] { "Colors" }
         )]
        [SwaggerResponse(StatusCodes.Status200OK, "Success - returns color", typeof(ColorsItem))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ProblemDetails))]
        public async Task<IActionResult> GetByNameAsync(
            [FromQuery, SwaggerParameter("Name of Color to return", Required = true)] string colorName)
        {

            ColorsItem _ColorsItem = await _ColorsService.GetByName(colorName);
            if (_ColorsItem == null)
            {
                return NotFound(new ProblemDetails { Status = 404, Title = "Not Found - {colorName}: " + colorName });
            }

            return Ok(_ColorsItem);
        }


        [Route("random")]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get random color",
            Description = "Returns random color.",
            OperationId = "GetRandomColor",
            Tags = new[] { "Colors" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Success - returns random color", typeof(ColorsItem))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ProblemDetails))]
        public async Task<IActionResult> RandomAsync()
        {
            List<ColorsItem> _ColorsList = await _ColorsService.GetAll();
            if (_ColorsList.Count == 0)
            {
                return NotFound(new ProblemDetails { Status = 404, Title = "Not Found - no colors exist"});
            }

            Random rnd = new Random();
            int rndInt = rnd.Next(_ColorsList.Count);

            return Ok(_ColorsList[rndInt]);
        }

        [Route("reset")]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Reset colors",
            Description = "Reset colors to default.",
            OperationId = "ResetColors",
            Tags = new[] { "Colors" }
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Success - colors reset", typeof(ColorsItem))]
        public async Task<IActionResult> ResetAsync()
        {

            await _ColorsService.Reset();

            List<ColorsItem> _ColorsList = await _ColorsService.GetAll();

            return Created("Colors/", _ColorsList);
        }

    }
}
