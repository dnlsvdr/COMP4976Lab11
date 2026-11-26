using GitHub.Copilot;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AI4SchoolAgent.Services;

public class AgentService
{
    private readonly StudentsAgentTools _toolService;

    public AgentService(StudentsAgentTools toolService)
    {
        _toolService = toolService;
    }

    // Create a Copilot client using PAT authentication
    private CopilotClient CreateClient()
{
    var token = Environment.GetEnvironmentVariable(
        "COPILOT_GITHUB_TOKEN");

    if (string.IsNullOrWhiteSpace(token))
    {
        throw new InvalidOperationException(
            "GitHub PAT is missing.");
    }

    return new CopilotClient(new CopilotClientOptions
    {
        LogLevel = CopilotLogLevel.Info,
        GitHubToken = token,
        UseLoggedInUser = false
    });
}

    // Retrieve available AI models
    public async Task<List<string>> GetModelsAsync()
{
    await using var client = CreateClient();

    await client.StartAsync();

    Console.WriteLine("Retrieving Copilot models...");

    var models = await client.ListModelsAsync();

    foreach (var model in models)
    {
        Console.WriteLine($"Available model: {model.Id}");
    }

    return models
        .Select(m => m.Id)
        .Where(m => m != "auto")
        .Distinct()
        .ToList();
}

    // Send a prompt using the selected model
    public async Task<string> AskAsync(
        string prompt,
        string selectedModel)
    {
        await using var copilotClient = CreateClient();

        await copilotClient.StartAsync();

        AIFunction searchTool = CopilotTool.DefineTool(
            _toolService.FindStudentsByAnyFilterAsync,

            toolOptions: new CopilotToolOptions
            {
                SkipPermission = true
            },

            factoryOptions: new AIFunctionFactoryOptions
            {
                Name = "find_students_by_any_filter",
                Description =
                    "Search students by name (first OR last), " +
                    "firstName, lastName, school, and/or birthYear. " +
                    "Supplied filters combine with AND; provide at least one. " +
                    "Text matching is case-insensitive, " +
                    "ignores surrounding input whitespace, " +
                    "and uses matchMode exact (default) or contains. " +
                    "Use the stored school name, e.g. Computing rather than School of Computing."
            }
        );

        var sessionConfig = new SessionConfig
        {
            Model = selectedModel,

            Tools = [searchTool],

            AvailableTools = ["find_students_by_any_filter"],

            SystemMessage = new SystemMessageConfig
            {
                Content = """
                    You are a helpful school assistant.
                    Use the registered student search tool
                    to answer questions about students.
                    Do not invent student records.
                    """
            }
        };

        AIAgent agent = copilotClient.AsAIAgent(
            sessionConfig: sessionConfig,
            name: "School Assistant"
        );

        AgentResponse response = await agent.RunAsync(prompt);

        return response.Text;
    }
}