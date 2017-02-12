using System.Linq;
using numl.Tests.Data;
using Xunit;

namespace numl.Tests.UnsupervisedTests
{
  [Trait("Category", "Unsupervised")]
  public class HierarchicalClusteringTests
  {
    [Fact]
    public void Cluster_Student()
    {
      var students = Student.GetData().Take(20).ToArray();
      HClusterModel cluster = new HClusterModel();
      Descriptor descriptor = Descriptor.Create<Student>();
      CentroidLinker linker = new CentroidLinker(new EuclidianDistance());
      Cluster root = cluster.Generate(descriptor, students, linker);
    }
  }
}