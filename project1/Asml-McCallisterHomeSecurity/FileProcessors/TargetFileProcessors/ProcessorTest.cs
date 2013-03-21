/*
 * ProcessorTest.cs
 * implements a NUnit unit test for the file processor classes
 * Written for CptS323 at Washington State University, Spring 2013
 * Written By: Chris Walters
 * Last Modified By: Chris Walters
 * Date Modified: March 14, 2013
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Targets;
using System.Xml;

namespace TargetFileProcessors
{
    [TestFixture]
    class ProcessorTest
    {
        [Test]
        public void IniTest()
        {
            FileProcessorFactory fact = FileProcessorFactory.GetInstance();
            FileProcessor proc = fact.Create("E:\\users\\chris\\documents\\GitHub\\testfiles\\good.ini");
            List<Target> _targets = proc.ProcessFile();
        }

        [Test]
        [ExpectedException(typeof(InvalidIniFormat))]
        public void IniTestReject()
        {
            FileProcessorFactory fact = FileProcessorFactory.GetInstance();
            FileProcessor proc = fact.Create("E:\\users\\chris\\documents\\GitHub\\testfiles\\rejects.ini");
            proc.ProcessFile();
        }

        [Test]
        public void XmlTest()
        {
            FileProcessorFactory fact = FileProcessorFactory.GetInstance();
            FileProcessor proc = fact.Create("E:\\users\\chris\\documents\\GitHub\\testfiles\\good.xml");
            List<Target> _targets = proc.ProcessFile();
            Assert.AreEqual(3, _targets.Count);
        }

        [Test]
        [ExpectedException(typeof(XmlException))]
        public void XmlTestReject()
        {
            FileProcessorFactory fact = FileProcessorFactory.GetInstance();
            FileProcessor proc = fact.Create("E:\\users\\chris\\documents\\GitHub\\testfiles\\shouldreject.xml");
            proc.ProcessFile();
        }
    }
}
