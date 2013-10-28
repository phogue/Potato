namespace Procon.Core {
    public interface IExecutableBase {
        CommandResultArgs RunPreview(Command command);

        CommandResultArgs RunHandler(Command command);

        CommandResultArgs RunExecuted(Command command);

        CommandResultArgs Execute(Command command);
    }
}
