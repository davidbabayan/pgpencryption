using (PgpCore.PGP pgp = new PgpCore.PGP())
{
    System.IO.Directory.CreateDirectory(@"C:\TEMP\Keys");
    pgp.GenerateKey(
        @"C:\TEMP\Keys\public.asc",
        @"C:\TEMP\Keys\private.asc",
        "email@email.com",
        "password");
}
System.Console.WriteLine("The keys are generated successfully!");
System.Console.ReadKey();