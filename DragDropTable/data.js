
var workitems = [];
workitems.push(new WorkItem("1233", "Feature", "Holiday Readiness - Infra Readiness", "1809-2", "Black Panthers", [{ item: "1234", UserStory: "Run ML on transaction data" }], new Date(2018, 1, 5), "open"));
workitems.push(new WorkItem("1237", "feature", "Retailer-PaaS Service Improvements", "1808-2", "Black Panthers", [], new Date(2018, 1, 5), "open"));

workitems.push(new WorkItem("1234", "Dependency", "Duplicate Check - Archive and purge transaction metadata", "1807-2", "DareDevils", [], new Date(2018, 1, 5), "open"));
workitems.push(new WorkItem("1238", "feature", "Holistic Transaction Volume Analysis", "1809-2", "DareDevils", [], new Date(2018, 1, 5), "open"));

workitems.push(new WorkItem("1235", "Dependency", "Make data setup ready for demo partner ContosoUS", "1808-1", "Incredibles", [{ item: "1234", UserStory: "Verify Archive data" }], new Date(2018, 1, 5), "open"));
workitems.push(new WorkItem("1239", "feature", "COM experience - Enhancements", "1809-1", "Incredibles", [{ item: "1231", UserStory: "API ML Algorithm" }, { item: "1235", UserStory: "Design ML Algorithm" }], new Date(2018, 1, 5), "open"));


workitems.push(new WorkItem("1236", "feature", "Batch and De-batch create correlation design", "1809-1", "Dauntless", [{ item: "1235", UserStory: "Make data setup" }], new Date(2018, 1, 5), "open"));

workitems.push(new WorkItem("1240", "feature", "Edifact Custom-String component padding", "1809-2", "Dauntless", [{ item: "1235", UserStory: "Create ContosoUS configuration" }], new Date(2018, 1, 5), "open"));
workitems.push(new WorkItem("1231", "Dependency", "Develop ML Algorithm", "1808-2", "Dauntless", [], new Date(2018, 1, 5), "open"));


for (var i = 0; i < workitems.length; i++) {
    workitems[i].appendWorkItem()
}

for (var i = 0; i < workitems.length; i++) {
    workitems[i].connect();
}
