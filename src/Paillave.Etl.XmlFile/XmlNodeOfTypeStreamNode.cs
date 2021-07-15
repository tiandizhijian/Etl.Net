﻿using Paillave.Etl.Core;
using Paillave.Etl.Reactive.Operators;
using Paillave.Etl.XmlFile.Core;

namespace Paillave.Etl.XmlFile
{
    public class XmlNodeOfTypeFileArgs<TOut>
    {
        public IStream<XmlNodeParsed> MainStream { get; set; }
        public string NodeDefinitionName { get; set; }
    }
    public class XmlNodeOfTypeStreamNode<TOut> : StreamNodeBase<TOut, IStream<TOut>, XmlNodeOfTypeFileArgs<TOut>>
    {
        public override ProcessImpact PerformanceImpact => ProcessImpact.Light;

        public override ProcessImpact MemoryFootPrint => ProcessImpact.Light;
        public XmlNodeOfTypeStreamNode(string name, XmlNodeOfTypeFileArgs<TOut> args) : base(name, args) { }
        protected override IStream<TOut> CreateOutputStream(XmlNodeOfTypeFileArgs<TOut> args)
        {
            var type = typeof(TOut);
            var obs = args.MainStream.Observable.Filter(i => i.Type == type);
            if (args.NodeDefinitionName != null)
                obs = obs.Filter(i => i.NodeDefinitionName == args.NodeDefinitionName);
            return CreateUnsortedStream(obs.Map(i => (TOut)i.Value));
        }
    }
    public class XmlNodeOfTypeCorrelatedStreamNode<TOut> : StreamNodeBase<Correlated<TOut>, IStream<Correlated<TOut>>, XmlNodeOfTypeFileArgs<TOut>>
    {
        public override ProcessImpact PerformanceImpact => ProcessImpact.Light;

        public override ProcessImpact MemoryFootPrint => ProcessImpact.Light;
        public XmlNodeOfTypeCorrelatedStreamNode(string name, XmlNodeOfTypeFileArgs<TOut> args) : base(name, args) { }
        protected override IStream<Correlated<TOut>> CreateOutputStream(XmlNodeOfTypeFileArgs<TOut> args)
        {
            var type = typeof(TOut);
            var obs = args.MainStream.Observable.Filter(i => i.Type == type);
            if (args.NodeDefinitionName != null)
                obs = obs.Filter(i => i.NodeDefinitionName == args.NodeDefinitionName);
            return CreateUnsortedStream(obs.Map(i => new Correlated<TOut>
            {
                CorrelationKeys = i.CorrelationKeys,
                Row = (TOut)i.Value
            }));
        }
    }
}
