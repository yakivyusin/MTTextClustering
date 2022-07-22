﻿using CorDeGen;
using MTTextClustering.Models;
using System;
using System.Linq;

namespace MTTextClustering.DataGenerators
{
    public class DataGenerator
    {
        public InputModel Generate(DataGeneratorModel model)
        {
            var corpusGenerator = new CorpusGenerator(model.TermsCount, ITermPresenter.Default);
            var corpus = corpusGenerator
                .GetCorpus()
                .Select(x => new Text(Guid.NewGuid(), x))
                .ToArray();

            return new InputModel(corpus, model.Method, model.Params);
        }
    }
}
