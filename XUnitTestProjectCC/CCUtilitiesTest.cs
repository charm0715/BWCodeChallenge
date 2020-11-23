using System;
using Xunit;
using CC;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XUnitTestProjectCC
{
    public class CCUtilitiesTest
    {
        [Theory]
        [InlineData("", "a")]
        [InlineData("a", "b" )]
        [InlineData("aa", "bb")]
        [InlineData("z", "aa")]
        [InlineData("zz", "aaa")]
        [InlineData("aaa", "bbb")]
        public void GetNextAlphaLine_ShouldReturnNext(string currentLetter, string expected)
        {
            //Act
            string actual = Utilities.GetNextAlphaLine(currentLetter);

            //Assert
            Assert.Equal(expected, actual);
            
        }


        [Theory]
        [InlineData("paragraphs", "2,4")]
        [InlineData("i", "1,3")]
        [InlineData("e.g.", "3")]
        [InlineData("mr.", "1")]
        [InlineData("mrs.", "3")]
        [InlineData("the", "2,2,5,5,5")]
        public void FindWordInSentences_ShouldFindSentence(string wordToFind, string expected)
        {
            //Arrange
            var sentences = new Dictionary<int, string>();
            sentences.Add(1, "this is what i learned from mr. jones about a paragraph");
            sentences.Add(2, "essays usually have multiple paragraphs that make claims to support a thesis statement, which is the central idea of the essay");
            sentences.Add(3, "i can now write topics on sports e.g. basketball, football, baseball and submit it to mrs. smith");
            sentences.Add(4, "paragraphs can begin with an indentation, or by missing a line out, and then starting again");
            sentences.Add(5, "this topic sentence of the paragraph tells the reader what the paragraph will be about");


            //Act
            string actual = Utilities.FindWordInSentences(sentences, wordToFind.ToLower());

            //Assert
            Assert.Equal(expected, actual);

        }

        [Theory]
        [InlineData("paragraphs", 5)]
        [InlineData("i", 2)]
        [InlineData("e.g", 1)]
        [InlineData("mr", 1)]
        [InlineData("mrs", 1)]
        [InlineData("the", 7)]
        [InlineData("sentences", 3)]
        public void CountWordOccurrences_ShouldCountwords(string wordToFind, int expected)
        {
            //Arrange
            string text = "This is what I learned from Mr. Jones about a paragraph. A paragraph is a group of words put together to form a group that is usually longer than a sentence. Paragraphs are often made up of several sentences. There are usually between three and eight sentences. Paragraphs can begin with an indentation, or by missing a line out, and then starting again. This makes it easier to see when one paragraph ends and another begins. In most organized forms of writing, such as essays, paragraphs contain a topic sentence. This topic sentence of the paragraph tells the reader what the paragraph will be about. Essays usually have multiple paragraphs that make claims to support a thesis statement, which is the central idea of the essay. Paragraphs may signal when the writer changes topics. Each paragraph may have a number of sentences, depending on the topic. I can now write topics on sports e.g. basketball, football, baseball and submit it to Mrs. Smith.";
            Regex _wordMatcher = new Regex(@"[^\p{L}]*\p{Z}[^\p{L}]*");
            var words = _wordMatcher.Split(text);


            //Act
            var dictionaryWords = Utilities.CountWordOccurrences(words);
            int actual = 0;

            if (dictionaryWords.ContainsKey(wordToFind))
                actual = dictionaryWords[wordToFind];
            
            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("This is what I learned from Mr.Jones about a paragraph.A paragraph is a group of words put together to form a group that is usually longer than a sentence.Paragraphs are often made up of several sentences.There are usually between three and eight sentences.Paragraphs can begin with an indentation, or by missing a line out, and then starting again.This makes it easier to see when one paragraph ends and another begins.In most organized forms of writing, such as essays, paragraphs contain a topic sentence.This topic sentence of the paragraph tells the reader what the paragraph will be about.Essays usually have multiple paragraphs that make claims to support a thesis statement, which is the central idea of the essay.Paragraphs may signal when the writer changes topics.Each paragraph may have a number of sentences, depending on the topic.I can now write topics on sports e.g.basketball, football, baseball and submit it to Mrs.Smith.", 13)]
        [InlineData("Each paragraph may have a number of sentences, depending on the topic.I can now write topics on sports e.g.basketball, football, baseball and submit it to Mrs.Smith. This is test; I think it is wrong. I am starting to count 12345. Then he said \"I am not worthy\". \"This is wrong!\". I am entirely worthy of praise!", 7)]
        [InlineData("In most organized forms of writing, such as essays, paragraphs contain a topic sentence.This topic sentence of the paragraph tells the reader what the paragraph will be about.Essays usually have multiple paragraphs that make claims to support a thesis statement, which is the central idea of the essay.Paragraphs may signal when the writer changes topics.Each paragraph may have a number of sentences, depending on the topic.I can now write topics on sports e.g.basketball, football, baseball and submit it to Mrs.Smith. This is test; I think it is wrong. I am starting to count 12345. Then he said \"I am not worthy\". \"This is wrong!\". I am entirely worthy of a better life!", 11)]
        [InlineData("123456, we all start counting this way. \"Alas I say to you!\" says Charm. We should all start thinking about the environment e.g. recycling. Mrs. Candor said the future is uncertain.", 4)]
        [InlineData("We offer prayers every 3:00 pm everyday for this.The environment is fragile; it is no longer an option to simply stand-by. While everyone else is shouting \"All lives matter!\". It is now the time to start doing something about it.", 5)]

        public void SplitIntoSentences_ShouldSplitCorrectly(string article, int expectedCount)
        {
            //Arrange
            Regex sentenceMatcher = new Regex(@"(?<!Mr?s?|\b[A-Z])\.\s*", RegexOptions.IgnoreCase);

            //Act
            var dictionaryResult = Utilities.SplitIntoSentences(article, sentenceMatcher);
            int actualCount = dictionaryResult.Count;

            //Assert
            Assert.Equal(expectedCount, actualCount);

        }

    }
}
