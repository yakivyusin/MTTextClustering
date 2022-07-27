using MTaaS.MetamorphicRelations;
using MTTextClustering.DataGenerators;
using MTTextClustering.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace MTTextClustering.Tests
{
    public class HierarchicalSingleLinkTests
    {
        private readonly static DataGeneratorModel _model = new(
            new Random().Next(450, 650),
            ClusteringMethods.SingleLink,
            new Dictionary<string, object>
            {
                ["threshold"] = 0.02
            });

        [Fact]
        [Trait("MR", "MR1")]
        public void ReverseTexts()
        {
            var dataGenerator = new DataGenerator();
            var input = dataGenerator.Generate(_model);
            var relation = new ReverseTextsMetamorphicRelation();

            Assert.True(relation.Validate(input));
        }

        [Fact]
        [Trait("MR", "MR2")]
        public void DuplicateCorpus()
        {
            var dataGenerator = new DataGenerator();
            var input = dataGenerator.Generate(_model);
            var relation = new DuplicateCorpusMetamorphicRelation();

            Assert.True(relation.Validate(input));
        }

        [Fact]
        [Trait("MR", "MR3")]
        public void DuplicateText()
        {
            var dataGenerator = new DataGenerator();
            var input = dataGenerator.Generate(_model);
            var relation = new DuplicateTextMetamorphicRelation();

            Assert.True(relation.Validate(input));
        }

        [Fact]
        [Trait("MR", "MR4")]
        public void ReverseTerms()
        {
            var dataGenerator = new DataGenerator();
            var input = dataGenerator.Generate(_model);
            var relation = new ReverseTermsMetamorphicRelation();

            Assert.True(relation.Validate(input));
        }

        [Fact]
        [Trait("MR", "MR5")]
        public void ReverseParagraphs()
        {
            var dataGenerator = new DataGenerator();
            var input = dataGenerator.Generate(_model);
            var relation = new ReverseParagraphsMetamorphicRelation();

            Assert.True(relation.Validate(input));
        }

        [Fact]
        [Trait("MR", "MR6")]
        public void ReverseParagraphTerms()
        {
            var dataGenerator = new DataGenerator();
            var input = dataGenerator.Generate(_model);
            var relation = new ReverseParagraphTermsMetamorphicRelation();

            Assert.True(relation.Validate(input));
        }

        [Fact]
        [Trait("MR", "MR7")]
        public void DoubleTerms()
        {
            var dataGenerator = new DataGenerator();
            var input = dataGenerator.Generate(_model);
            var relation = new DoubleTermsMetamorphicRelation();

            Assert.True(relation.Validate(input));
        }
    }
}
