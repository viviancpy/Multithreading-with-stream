# Multithreading-with-stream
Display stream with high frequency update to screen using asychronous programming techniques

I was given a few questions to solve for analysing data from a high frequency updated source of data. This is the first question which required a proof of concept:

 - The application needs to continually read text data from the Stream(updated with high frequency);
 - The application needs to provide in real time (i.e. as you read it from the Stream and display with low latency) the following information about the text: 
 
1. Total Number of Characters and words 
2. The 5 Largest and 5 smallest word 
3. 10 Most frequently used words
4. A list showing all the characters used in the text and the number of times they appear sorted in descending order.


I did this proof of concept using:
1. Basic asynchronous programming technique (async/await and Task)
2. DataFlow as the multithreading stream strategy (I choose not to use classic Parallel programming strategies for having a nicer consumer producer pattern)
3. Thread switching available in WPF
