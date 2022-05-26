using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiffingAPI.Tests
{
    public class Tests
    {

        private readonly HttpClient _client;
        private readonly string host = "localhost:5000";
        private readonly string baseUrl = "/v1/diff/";
        private readonly string protocol = "http://";

        public string url;

        public Tests()
        {
            _client = new HttpClient();
        }

        [SetUp]
        public void Setup()
        {
            url = protocol + host + baseUrl;
        }

        //Testing response when requesting diff with no data
        [Test]
        [TestCase(123)]
        [TestCase(234)]
        [TestCase(345)]
        [TestCase(456)]
        public async Task TestGetDiffWithNoData(int diffId)
        {
            HttpResponseMessage response = await _client.GetAsync(url + diffId);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

        }

        //Testing response when sending left item for comparing
        [Test]
        [TestCase(1, "AAAAAA==")]
        [TestCase(2, "AAA=")]
        public async Task TestPutLeft(int diffId, string sdata)
        {
            var data = new { data = sdata };

            HttpResponseMessage response = await _client.PutAsync(url + diffId + "/left", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        //Testing response when sending right item for comparing
        [Test]
        [TestCase(1, "AQABAQ==")]
        [TestCase(2, "AQA=")]
        public async Task TestPutRight(int diffId, string sdata)
        {
            var data = new { data = sdata };

            HttpResponseMessage response = await _client.PutAsync(url + diffId + "/right", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        //Testing response when sending null instead of data
        [Test]
        [TestCase(1, "/left")]
        [TestCase(2, "/left")]
        [TestCase(1, "/right")]
        public async Task TestPutNull(int diffId, string endpoint)
        {
            var data = new { data = "null" };

            HttpResponseMessage response = await _client.PutAsync(url + diffId + endpoint, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        //Testing response when requesting diff with not enough data
        [Test]
        [TestCase(6, "/left")]
        [TestCase(8, "/left")]
        public async Task TestGetDiffWithPartialData(int diffId, string endpoint)
        {
            byte[] strBytes = Encoding.UTF8.GetBytes(new String("Sample String"));
            string sdata = Convert.ToBase64String(strBytes);
            var data = new { data = sdata };

            await _client.PutAsync(url + diffId + endpoint, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));

            HttpResponseMessage response = await _client.GetAsync(url + diffId);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        //Testing response when requesting diff for same values
        [Test]
        [TestCase(1, "AAAAAA==")]
        [TestCase(2, "AAA=")]
        public async Task TestGetDiffWhenEqual(int diffId, string sdata)
        {
            var data = new { data = sdata };
            var expectedResponse = new { diffResultType = "Equals" };

            await _client.PutAsync(url + diffId + "/left", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
            await _client.PutAsync(url + diffId + "/right", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));

            HttpResponseMessage response = await _client.GetAsync(url + diffId);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseData = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(JsonConvert.SerializeObject(expectedResponse), responseData, "Response body that is not expected.");
        }

        //Testing response when requesting diff for different values that are same length
        [Test]
        [TestCase(1, "AAAAAA==", "AAA=")]
        [TestCase(2, "AQA=", "AQABAQ==")]
        public async Task TestGetDiffWhenLengthsNotEqual(int diffId, string leftData, string rightData)
        {
            var left = new { data = leftData };
            var right = new { data = rightData };
            var expectedResponse = new { diffResultType = "SizeDoNotMatch" };

            await _client.PutAsync(url + diffId + "/left", new StringContent(JsonConvert.SerializeObject(left), Encoding.UTF8, "application/json"));
            await _client.PutAsync(url + diffId + "/right", new StringContent(JsonConvert.SerializeObject(right), Encoding.UTF8, "application/json"));

            HttpResponseMessage response = await _client.GetAsync(url + diffId);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseData = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(JsonConvert.SerializeObject(expectedResponse), responseData, "Response body that is not expected.");
        }

        //Helper structure used for storing all diff offsets and lengths
        public struct diff
        {
            public int offset { get; set; }
            public int length { get; set; }

        }

        //Helper method to create array of all diffs from given offsets and lengths
        public List<diff> CreateDiffArray(int[] offsets, int[] lengths)
        {
            if (offsets.Length != lengths.Length)
            {
                return null;
            }

            List<diff> diffArray = new List<diff>();

            for (int i = 0; i < offsets.Length; i++)
            {
                diffArray.Add(new diff { offset = offsets[i], length = lengths[i] });
            }

            return diffArray;
        }

        //Testing response when requesting diff for values that are different length
        [Test]
        [TestCase(4, "AAAAAA==", "AQABAQ==", new int[] { 0, 2 }, new int[] { 1, 2 })]
        [TestCase(5, "AAA=", "AQA=", new int[] { 0, }, new int[] { 1, })]
        public async Task TestGetDiffWhenLengthsAreEqual(int diffId, string leftData, string rightData, int[] diffOffsets, int[] diffLengths)
        {
            var left = new { data = leftData };
            var right = new { data = rightData };

            string resultType = "ContentDoNotMatch";
            List<diff> diffs = CreateDiffArray(diffOffsets, diffLengths);

            var expectedResponse = new { diffResultType = resultType, diffs };


            await _client.PutAsync(url + diffId + "/left", new StringContent(JsonConvert.SerializeObject(left), Encoding.UTF8, "application/json"));
            await _client.PutAsync(url + diffId + "/right", new StringContent(JsonConvert.SerializeObject(right), Encoding.UTF8, "application/json"));

            HttpResponseMessage response = await _client.GetAsync(url + diffId);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseData = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(JsonConvert.SerializeObject(expectedResponse), responseData, "Response body that is not expected.");
        }
    }
}