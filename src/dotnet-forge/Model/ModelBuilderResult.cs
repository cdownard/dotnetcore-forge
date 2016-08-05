using Microsoft.CodeAnalysis.Emit;

namespace Forge
{
    public sealed class ModelBuilderResult
    {
        public string Output { get; }
        public EmitResult CompileResult { get; }

        public ModelBuilderResult(string output, EmitResult compileResult)
        {
            Output = output;
            CompileResult = compileResult;
        }
    }
}