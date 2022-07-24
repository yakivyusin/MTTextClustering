using CorDeGen;
using MTaaS.Attributes;
using MTTextClustering.Models;
using System;
using System.Linq;

namespace MTTextClustering.DataGenerators
{
    [DataGenerator("ReverseTexts")]
    [DataGenerator("DuplicateText")]
    [DataGenerator("DuplicateCorpus")]
    [DataGenerator("ReverseTerms")]
    [DataGenerator("ReverseParagraphs")]
    [DataGenerator("DoubleTerms")]
    [DataGenerator("ReverseParagraphTerms")]
    [DataGenerator("AddOutlineText")]
    [DataGenerator("AddOutlineTextDBI")]
    [DataGenerator("SetKToDocumentsCount")]
    [DataGenerator("DecreaseCorpus")]
    public class DataGenerator
    {
        public InputModel Generate(DataGeneratorModel model)
        {
            var corpusGenerator = new CorpusGenerator(model.TermsCount, ITermPresenter.Default);
            var corpus = corpusGenerator.GetCorpus();
            var guids = Enumerable.Range(1, corpus.Length)
                .Select(x => new Guid(x, 22, 33, new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 }))
                .OrderByDescending(x => x)
                .ToArray();

            var texts = corpus
                .Select((x, i) => new Text(guids[i], x))
                .ToArray();

            return new InputModel(texts, model.Method, model.Params);
        }
    }
}
