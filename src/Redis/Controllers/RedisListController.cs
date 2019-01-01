﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Detectors.Redis.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Detectors.Redis.Controllers
{
    [Route("redis/connection/{connectionId}/db/{dbId}/list/{key}")]
    [Route("redis/connection/{connectionId}/list/{key}")]
    public class RedisListController : Controller
    {
        private readonly RedisConnectionConfigCollection _configuration;
        private readonly ILogger _logger;
        public RedisListController(RedisConnectionConfigCollection configuration, ILogger<RedisListController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("length")]
        [HttpGet("length.{format}")]
        public IActionResult GetLength(string connectionId, string key, int dbId = -1)
        {
            try
            {
                using (var redis = _configuration.BuildMultiplexer(connectionId))
                {
                    if (redis == null)
                        return NotFound();

                    var result = redis.GetDatabase(dbId).ListLength(key);
                    return Ok(result);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError("redis list length exception message", exception);
                return BadRequest(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            }
        }

        [HttpGet("index/{index}")]
        [HttpGet("index/{index}.{format}")]
        public IActionResult GetIndex(string connectionId, string key, long index, int dbId = -1)
        {
            using (var redis = _configuration.BuildMultiplexer(connectionId))
            {
                if (redis == null)
                    return NotFound();

                var result = redis.GetDatabase(dbId).ListGetByIndex(key, index);
                return Ok((byte[]) result);
            }
        }
        
        [HttpGet("index/{index}/string")]
        [HttpGet("index/{index}/string.{format}")]
        public IActionResult GetIndexString(string connectionId, string key, long index, int dbId = -1)
        {
            using (var redis = _configuration.BuildMultiplexer(connectionId))
            {
                if (redis == null)
                    return NotFound();

                var result = redis.GetDatabase(dbId).ListGetByIndex(key, index);
                return Ok((string) result);
            }
        }
        
        [HttpGet("range")]
        [HttpGet("range.{format}")]
        public IActionResult GetRange(string connectionId, string key, long start = 0L, long stop = -1L, int dbId = -1)
        {
            using (var redis = _configuration.BuildMultiplexer(connectionId))
            {
                if (redis == null)
                    return NotFound();

                var result = redis.GetDatabase(dbId).ListRange(key, start, stop);
                return Ok(result.Select(v => (byte[])v));
            }
        }
        
        [HttpGet("range/string")]
        [HttpGet("range/string{.format}")]
        public IActionResult GetRangeString(string connectionId, string key, long start = 0L, long stop = -1L, int dbId = -1)
        {
            using (var redis = _configuration.BuildMultiplexer(connectionId))
            {
                if (redis == null)
                    return NotFound();

                var result = redis.GetDatabase(dbId).ListRange(key, start, stop);
                return Ok(result.Select(v => (string)v));
            }
        }
    }
}