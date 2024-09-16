# Introduction 
This is a sample project for handling currency exchange funtionalities. Below are the 3 enpoints exposed
1. FetchLatestRatesForBaseCurrency
2. ConvertCurrency
3. HistoricalRates

# Getting Started
To setup this project, follow below steps
1. Install .Net 6.0 
2. Run "Update-Package" command in visual studio package manager console to restore packages
3. Rebuild the solution

# Build and Test
To run the project or testcase follow below steps
1. Run the project
2. If need to execute testcases, navigate to Test menu and click Run all tests

# Enhancements 
Below are some of the suggested enhancements which can be made for this project

1. Switch to onion architecture pattern for better management and reusability
2. Using redis to manage cache to persist data even if appication restarts or crashes
3. As of now only positive testcases are added. Need for more test cases for mocking negative cases and we as for other layers
4. Docker setup to manage deployment and scalability
5. Handle the pagination for both outer as well as inner dictionaries