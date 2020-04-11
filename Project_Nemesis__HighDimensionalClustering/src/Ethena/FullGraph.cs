using System;
using System.Collections.Generic;
using System.Text;

namespace Chaos.src.Ethena
{
    public struct Edge
    {
        TextAnalysis v;
        TextAnalysis u;
        public Edge(TextAnalysis a, TextAnalysis b)
        {
            v = a;
            u = b; 
        }
    }

    class FullGraph
    {
        
        protected ISet<TextAnalysis> vertices;
        protected IDictionary<double, Edge> all_edges = new SortedDictionary<double, Edge>();
        int n;
        public FullGraph(ISet<TextAnalysis> arg)
        {
            vertices = arg;
            TextAnalysis[] vertexList = new TextAnalysis[arg.Count];
            n = arg.Count; 
            vertices.CopyTo(vertexList, 0);
            
            for (int I = 0; I < vertexList.Length; I++)
            for (int J = I + 1; J < vertexList.Length; J++)
            {
                all_edges[TextAnalysis.Dis(vertexList[I], vertexList[J])] 
                    = new Edge(vertexList[I], vertexList[J]);
            }

        }



    }


}
