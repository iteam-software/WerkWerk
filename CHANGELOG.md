# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unstable]

### Changed

- WorkContext.Items is now a property.

### Added

- Job model default values.
- WorkContext.Items to store arbitrary data in the job execution pipeline.
- QOL method for configuring the Job entity
- Job checksum so jobs can be easily compared based on the data they carry.
- Job repository method to fetch all jobs with the given name and checksum

## [1.0.0] - 2021-05-04

### Added

- CI/CD for the project
- Initial codebase

[unstable]: https://github.com/iteam-consulting/WerkWerk/compare/1.0.0...HEAD
[1.0.0]: https://github.com/iteam-consulting/WerkWerk/releases/tag/1.0.0
