if (args.Length == 0)
{
	Console.WriteLine("Usage: dotnet run <url>");
	return;
}

var client = new HttpClient();

await Parallel.ForEachAsync(Enumerable.Range(0, 100), async (_, _) =>
{
	var response = await client.GetAsync(args[0]);

	var result = await response.Content.ReadAsStringAsync();

	Console.WriteLine(result);
});
