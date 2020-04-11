using System;
using System.Collections.Generic;
using System.Text;

namespace Chaos.src.Ethena
{
    public struct Edge
    {
        public TextAnalysis v;
        public  TextAnalysis u;
        public Edge(TextAnalysis a, TextAnalysis b)
        {
            v = a;
            u = b; 
        }
    }

    public class FullGraph
    {
        
        protected TextAnalysis[] vertices;
        public IDictionary<double, Edge> all_edges {get; protected set;}
        int n;

        public FullGraph(ISet<TextAnalysis> arg)
        {
            TextAnalysis[] vertexList = new TextAnalysis[arg.Count];
            n = arg.Count;
            arg.CopyTo(vertexList, 0);

            vertices = vertexList; // set up the field. 

            all_edges = new SortedDictionary<double, Edge>(); 
            for (int I = 0; I < vertexList.Length; I++)
            for (int J = I + 1; J < vertexList.Length; J++)
            {
                all_edges[TextAnalysis.Dis(vertexList[I], vertexList[J])] 
                    = new Edge(vertexList[I], vertexList[J]);
            }
        }



        /// <summary>
        /// Get a list of stings, representing all the weight of the edges 
        /// </summary>
        /// <returns></returns>
        public string DistanceList()
        {
            StringBuilder sb = new StringBuilder();
            foreach(KeyValuePair<double, Edge> kvp in all_edges)
            {
                // sb.Append($"{kvp.Key}->{{ {kvp.Value.u.file_name}, {kvp.Value.v.file_name} }} \n");
                sb.Append(kvp.Key + "\n");
            }
            return sb.ToString();
        }

        public int VertexCount()
        {
            return vertices.Length;
        }



    }


}
