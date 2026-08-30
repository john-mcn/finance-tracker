# Project Structure
```
├── docs
├── README.md
├── src
│   └── FinanceTracker.Application
│   └── FinanceTracker.Domain
│   └── FinanceTracker.CLI
└── tests
    └── FinanceTracker.Tests
```
- [(inspiration)](https://medium.com/@orbens/the-ultimate-guide-to-structuring-scalable-net-projects-from-startup-to-enterprise-c72dae562d1b)

Domain -> (Repository -> Service) -> CLI

```
RecurringTransactionSource --(RecurrenceRule)-> Transaction
```
`Transaction` model representes a *historical* instance

`RecurringTransactionSource` model represents a framework of a recurring `Transaction` using a `RecurrenceRule` (rather than being a recurring historical 'log')
- `RecurrenceRule` model represents a rule for autonomously generating (`Transaction`s) at standard intervals.