# Breaking the Sorting Barrier — Benchmark Results

**Generated:** 2026-03-19 09:47:11
**Suite:** Full Comparison Suite
**Runtime:** .NET 10.0.5
**OS:** Microsoft Windows NT 10.0.26200.0
**Machine:** LAPTOP-IVNV7JPF (16 logical cores)

## Summary

- **Total test configurations:** 28
- **Correctness (BMSSP matches Dijkstra):** 28/28 (100.0%)
- **Status:** ✅ ALL PASS

## Detailed Results

| Graph Type | V | E | Algorithm | Time (ms) | Edge Relaxations | Heap Ops | Match |
|:-----------|----:|----:|:----------|----------:|-----------------:|---------:|:-----:|
| Complete(n=10) | 10 | 90 | Dijkstra | 0.005 | 90 | 29 | — |
| Complete(n=10) | 10 | 90 | BMSSP | 0.029 | 360 | 44 | ✅ |
| Complete(n=100) | 100 | 9900 | Dijkstra | 0.207 | 9,900 | 486 | — |
| Complete(n=100) | 100 | 9900 | BMSSP | 0.998 | 41,679 | 367 | ✅ |
| Complete(n=50) | 50 | 2450 | Dijkstra | 0.064 | 2,450 | 227 | — |
| Complete(n=50) | 50 | 2450 | BMSSP | 0.268 | 8,526 | 185 | ✅ |
| Grid(n=100) | 100 | 180 | Dijkstra | 0.037 | 180 | 219 | — |
| Grid(n=100) | 100 | 180 | BMSSP | 0.180 | 735 | 222 | ✅ |
| Grid(n=10000) | 10000 | 19800 | Dijkstra | 6.634 | 19,800 | 21,934 | — |
| Grid(n=10000) | 10000 | 19800 | BMSSP | 12.766 | 50,368 | 5,179 | ✅ |
| Grid(n=1000000) | 1000000 | 1998000 | Dijkstra | 907.253 | 1,998,000 | 2,197,802 | — |
| Grid(n=1000000) | 1000000 | 1998000 | BMSSP | 1476.630 | 4,468,284 | 218,696 | ✅ |
| Grid(n=2500) | 2500 | 4900 | Dijkstra | 1.283 | 4,900 | 5,503 | — |
| Grid(n=2500) | 2500 | 4900 | BMSSP | 3.022 | 14,721 | 2,440 | ✅ |
| Grid(n=250000) | 250000 | 499000 | Dijkstra | 222.922 | 499,000 | 549,296 | — |
| Grid(n=250000) | 250000 | 499000 | BMSSP | 319.367 | 1,113,731 | 54,348 | ✅ |
| Grid(n=40000) | 40000 | 79600 | Dijkstra | 31.908 | 79,600 | 87,880 | — |
| Grid(n=40000) | 40000 | 79600 | BMSSP | 71.285 | 202,955 | 20,734 | ✅ |
| Grid(n=900) | 900 | 1740 | Dijkstra | 0.445 | 1,740 | 1,968 | — |
| Grid(n=900) | 900 | 1740 | BMSSP | 1.082 | 5,163 | 846 | ✅ |
| Grid(n=90000) | 90000 | 179400 | Dijkstra | 72.420 | 179,400 | 197,791 | — |
| Grid(n=90000) | 90000 | 179400 | BMSSP | 129.106 | 428,390 | 32,689 | ✅ |
| LinearChain(n=10) | 10 | 9 | Dijkstra | 0.011 | 9 | 20 | — |
| LinearChain(n=10) | 10 | 9 | BMSSP | 0.037 | 35 | 46 | ✅ |
| LinearChain(n=100) | 100 | 99 | Dijkstra | 0.019 | 99 | 200 | — |
| LinearChain(n=100) | 100 | 99 | BMSSP | 0.340 | 494 | 496 | ✅ |
| LinearChain(n=1000) | 1000 | 999 | Dijkstra | 0.169 | 999 | 2,000 | — |
| LinearChain(n=1000) | 1000 | 999 | BMSSP | 1.901 | 4,495 | 3,497 | ✅ |
| LinearChain(n=10000) | 10000 | 9999 | Dijkstra | 1.608 | 9,999 | 20,000 | — |
| LinearChain(n=10000) | 10000 | 9999 | BMSSP | 16.264 | 44,995 | 34,997 | ✅ |
| LinearChain(n=5000) | 5000 | 4999 | Dijkstra | 0.883 | 4,999 | 10,000 | — |
| LinearChain(n=5000) | 5000 | 4999 | BMSSP | 8.934 | 22,495 | 17,497 | ✅ |
| RandomSparse(n=10) | 10 | 28 | Dijkstra | 0.007 | 27 | 22 | — |
| RandomSparse(n=10) | 10 | 28 | BMSSP | 0.045 | 92 | 28 | ✅ |
| RandomSparse(n=100) | 100 | 296 | Dijkstra | 0.076 | 296 | 220 | — |
| RandomSparse(n=100) | 100 | 296 | BMSSP | 0.461 | 1,200 | 221 | ✅ |
| RandomSparse(n=1000) | 1000 | 2997 | Dijkstra | 1.112 | 2,997 | 2,198 | — |
| RandomSparse(n=1000) | 1000 | 2997 | BMSSP | 2.958 | 6,507 | 319 | ✅ |
| RandomSparse(n=10000) | 10000 | 29997 | Dijkstra | 15.436 | 29,997 | 22,046 | — |
| RandomSparse(n=10000) | 10000 | 29997 | BMSSP | 37.410 | 49,730 | 1,369 | ✅ |
| RandomSparse(n=100000) | 100000 | 299998 | Dijkstra | 187.115 | 299,998 | 219,882 | — |
| RandomSparse(n=100000) | 100000 | 299998 | BMSSP | 303.149 | 403,574 | 7,387 | ✅ |
| RandomSparse(n=5000) | 5000 | 14996 | Dijkstra | 6.921 | 14,996 | 11,010 | — |
| RandomSparse(n=5000) | 5000 | 14996 | BMSSP | 16.623 | 27,882 | 719 | ✅ |
| RandomSparse(n=50000) | 50000 | 149998 | Dijkstra | 78.356 | 149,998 | 110,209 | — |
| RandomSparse(n=50000) | 50000 | 149998 | BMSSP | 234.347 | 226,806 | 3,856 | ✅ |
| RandomSparse(n=500000) | 500000 | 1499997 | Dijkstra | 1278.239 | 1,499,997 | 1,100,457 | — |
| RandomSparse(n=500000) | 500000 | 1499997 | BMSSP | 1828.816 | 1,914,461 | 22,496 | ✅ |
| Star(n=10) | 10 | 9 | Dijkstra | 0.003 | 9 | 20 | — |
| Star(n=10) | 10 | 9 | BMSSP | 0.033 | 27 | 30 | ✅ |
| Star(n=100) | 100 | 99 | Dijkstra | 0.083 | 99 | 200 | — |
| Star(n=100) | 100 | 99 | BMSSP | 0.360 | 396 | 300 | ✅ |
| Star(n=1000) | 1000 | 999 | Dijkstra | 0.856 | 999 | 2,000 | — |
| Star(n=1000) | 1000 | 999 | BMSSP | 1.809 | 3,996 | 1,063 | ✅ |
| Star(n=10000) | 10000 | 9999 | Dijkstra | 8.114 | 9,999 | 20,000 | — |
| Star(n=10000) | 10000 | 9999 | BMSSP | 14.611 | 39,996 | 10,127 | ✅ |

## Performance Comparison (BMSSP vs Dijkstra)

| Graph Type | V | Dijkstra (ms) | BMSSP (ms) | Speedup | Dijkstra Relaxations | BMSSP Relaxations | Relaxation Ratio |
|:-----------|----:|--------------:|-----------:|--------:|---------------------:|------------------:|-----------------:|
| LinearChain(n=10) | 10 | 0.011 | 0.037 | 3.27x slower | 9 | 35 | 3.89x |
| RandomSparse(n=10) | 10 | 0.007 | 0.045 | 6.14x slower | 27 | 92 | 3.41x |
| Star(n=10) | 10 | 0.003 | 0.033 | 9.88x slower | 9 | 27 | 3.00x |
| Complete(n=10) | 10 | 0.005 | 0.029 | 5.54x slower | 90 | 360 | 4.00x |
| Complete(n=50) | 50 | 0.064 | 0.268 | 4.22x slower | 2,450 | 8,526 | 3.48x |
| LinearChain(n=100) | 100 | 0.019 | 0.340 | 17.89x slower | 99 | 494 | 4.99x |
| RandomSparse(n=100) | 100 | 0.076 | 0.461 | 6.06x slower | 296 | 1,200 | 4.05x |
| Grid(n=100) | 100 | 0.037 | 0.180 | 4.79x slower | 180 | 735 | 4.08x |
| Star(n=100) | 100 | 0.083 | 0.360 | 4.32x slower | 99 | 396 | 4.00x |
| Complete(n=100) | 100 | 0.207 | 0.998 | 4.82x slower | 9,900 | 41,679 | 4.21x |
| Grid(n=900) | 900 | 0.445 | 1.082 | 2.43x slower | 1,740 | 5,163 | 2.97x |
| LinearChain(n=1000) | 1000 | 0.169 | 1.901 | 11.27x slower | 999 | 4,495 | 4.50x |
| RandomSparse(n=1000) | 1000 | 1.112 | 2.958 | 2.66x slower | 2,997 | 6,507 | 2.17x |
| Star(n=1000) | 1000 | 0.856 | 1.809 | 2.11x slower | 999 | 3,996 | 4.00x |
| Grid(n=2500) | 2500 | 1.283 | 3.022 | 2.35x slower | 4,900 | 14,721 | 3.00x |
| LinearChain(n=5000) | 5000 | 0.883 | 8.934 | 10.11x slower | 4,999 | 22,495 | 4.50x |
| RandomSparse(n=5000) | 5000 | 6.921 | 16.623 | 2.40x slower | 14,996 | 27,882 | 1.86x |
| LinearChain(n=10000) | 10000 | 1.608 | 16.264 | 10.12x slower | 9,999 | 44,995 | 4.50x |
| RandomSparse(n=10000) | 10000 | 15.436 | 37.410 | 2.42x slower | 29,997 | 49,730 | 1.66x |
| Grid(n=10000) | 10000 | 6.634 | 12.766 | 1.92x slower | 19,800 | 50,368 | 2.54x |
| Star(n=10000) | 10000 | 8.114 | 14.611 | 1.80x slower | 9,999 | 39,996 | 4.00x |
| Grid(n=40000) | 40000 | 31.908 | 71.285 | 2.23x slower | 79,600 | 202,955 | 2.55x |
| RandomSparse(n=50000) | 50000 | 78.356 | 234.347 | 2.99x slower | 149,998 | 226,806 | 1.51x |
| Grid(n=90000) | 90000 | 72.420 | 129.106 | 1.78x slower | 179,400 | 428,390 | 2.39x |
| RandomSparse(n=100000) | 100000 | 187.115 | 303.149 | 1.62x slower | 299,998 | 403,574 | 1.35x |
| Grid(n=250000) | 250000 | 222.922 | 319.367 | 1.43x slower | 499,000 | 1,113,731 | 2.23x |
| RandomSparse(n=500000) | 500000 | 1278.239 | 1828.816 | 1.43x slower | 1,499,997 | 1,914,461 | 1.28x |
| Grid(n=1000000) | 1000000 | 907.253 | 1476.630 | 1.63x slower | 1,998,000 | 4,468,284 | 2.24x |

## Correctness Verification

✅ **All BMSSP results match Dijkstra's output within floating-point tolerance (1e-9).**

This confirms that the BMSSP algorithm correctly computes single-source shortest paths
for all tested graph types and sizes.

## Algorithm Notes

- **Dijkstra**: Classic O(m log n) with binary min-heap. The gold-standard baseline.
- **BMSSP**: Implementation of the algorithm from *"Breaking the Sorting Barrier for
  Directed Single-Source Shortest Paths"* (Duan, Mao, Mao, Shu, Yin — STOC 2025).
  Theoretical complexity: O(m · log^(2/3)(n)) in the comparison-addition model.

### Key Observations

The BMSSP algorithm's theoretical advantage (log^(2/3)(n) vs log(n)) is asymptotic.
For practical graph sizes (n < 10^6), the constant factors and overhead of the recursive
decomposition, FindPivots, and partition data structure may outweigh the theoretical gain.
The primary value of this implementation is **correctness verification** — confirming that
the algorithm produces identical shortest-path distances to Dijkstra across diverse graph families.

