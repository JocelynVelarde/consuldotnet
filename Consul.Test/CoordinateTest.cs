// -----------------------------------------------------------------------
//  <copyright file="CoordinateTest.cs" company="PlayFab Inc">
//    Copyright 2015 PlayFab Inc.
//    Copyright 2020 G-Research Limited
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Consul.Test
{
    public class CoordinateTest : BaseFixture
    {
        [Fact]
        public async Task Coordinate_GetDatacenters()
        {
            var info = await _client.Agent.Self();

            var datacenters = await _client.Coordinate.Datacenters();

            Assert.NotNull(datacenters.Response);
            Assert.True(datacenters.Response.Length > 0);
        }

        [Fact]
        public async Task Coordinate_GetNodes()
        {
            var info = await _client.Agent.Self();

            var nodes = await _client.Coordinate.Nodes();

            // There's not a good way to populate coordinates without
            // waiting for them to calculate and update, so the best
            // we can do is call the endpoint and make sure we don't
            // get an error. - from offical API.
            Assert.NotNull(nodes);
        }

        [Fact]
        public async Task Coordinate_GetNode()
        {
            var info = await _client.Agent.Self();
            var nodesResult = await _client.Coordinate.Nodes();

            Assert.NotNull(nodesResult);
            Assert.NotEmpty(nodesResult.Response);

            var nodes = nodesResult.Response;

            var firstNode = nodes[0];

            var nodeDetailsResult = await _client.Coordinate.Node(firstNode.Node);

            Assert.NotNull(nodeDetailsResult);
            Assert.NotEmpty(nodeDetailsResult.Response);

            var nodeDetails = nodeDetailsResult.Response;

            Assert.IsType<CoordinateEntry[]>(nodeDetails);
            Assert.NotEmpty(nodeDetails);
        }

        [Fact]
        public async Task Coordinate_Update()
        {
            var info = await _client.Agent.Self();
            var node = "foob";
            var registerNode = await _client.Catalog.Register(new CatalogRegistration()
            {
                Node = node,
                Address = "1.1.1.1"
            }, default);

            Assert.NotNull(registerNode);
            
            var newCoordinate = new SerfCoordinate
            {
                Vec = new List<double> { 1.0, 2.0, 3.0 },
                Error = 0.5,
                Adjustment = 0.2,
                Height = 0.5,
 
            };

            var entry = new CoordinateEntry
            {
                Node = node,
                Segment = "",
                Coord = newCoordinate
            };

            var writeOptions = new WriteOptions();
            var updateResult = await _client.Coordinate.Update(entry, null);
            Assert.NotNull(updateResult);
        }
    }
}
