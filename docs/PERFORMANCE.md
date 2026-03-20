<p align="center">
  <img src="https://img.shields.io/badge/Benchmark-Performance_Dashboard-FF6B6B?style=for-the-badge&logo=speedtest&logoColor=white" alt="Performance Dashboard"/>
  <img src="https://img.shields.io/badge/Graphs_Tested-29_Configs-2962FF?style=for-the-badge" alt="29 Configs"/>
  <img src="https://img.shields.io/badge/Max_Scale-1M_Vertices-00C853?style=for-the-badge" alt="1M Vertices"/>
</p>

<h1 align="center">📊 Performance Dashboard</h1>

<p align="center">
  <strong>Three-way comparison: Dijkstra vs BMSSP vs BucketScan</strong><br/>
  <sub>All data from actual benchmark runs on .NET 10.0.5 | Windows | 16 logical cores</sub>
</p>

---

## 🏆 Executive Summary

<table>
<tr>
<td width="33%" align="center">

### Fastest Algorithm
```
      ⚡
    ╱    ╲
  ╱ Bucket ╲
 ╱   Scan   ╲
╱_____________╲
```
**BucketScan** wins on speed
for n ≥ 10,000

</td>
<td width="33%" align="center">

### Fewest Heap Ops
```
      🎯
    ╱    ╲
  ╱       ╲
 ╱  BMSSP  ╲
╱___________╲
```
**BMSSP** wins on heap ops
up to 57× fewer

</td>
<td width="33%" align="center">

### Best All-Around
```
      ⚡
    ╱    ╲
  ╱ Bucket ╲
 ╱   Scan   ╲
╱_____________╲
```
**BucketScan** — wins on
*both* speed AND heap ops

</td>
</tr>
</table>

---

## 📈 Speed Scaling — RandomSparse Graphs

> These are the most representative of real-world graphs (sparse, random structure).

```
Wall-clock time scaling (lower is better):

n = 10K   │ Dijkstra ████████░░░░░░░░░░░░  22.7 ms
          │ BMSSP    ████████████████████░  64.4 ms  (2.8× slower)
          │ BktScan  ███████░░░░░░░░░░░░░  20.9 ms  (1.09× faster) 🏆
          │
n = 50K   │ Dijkstra ████████░░░░░░░░░░░░  127.9 ms
          │ BMSSP    ████████████████████░  287.2 ms  (2.2× slower)
          │ BktScan  ██████░░░░░░░░░░░░░░  87.1 ms   (1.47× faster) 🏆
          │
n = 100K  │ Dijkstra ████████░░░░░░░░░░░░  164.9 ms
          │ BMSSP    ████████████████████░  339.9 ms  (2.1× slower)
          │ BktScan  ████████░░░░░░░░░░░░  163.6 ms  (1.01× faster) 🏆
          │
n = 500K  │ Dijkstra ████████░░░░░░░░░░░░  1,202.9 ms
          │ BMSSP    ████████████████░░░░░  1,893.2 ms  (1.6× slower)
          │ BktScan  ██████░░░░░░░░░░░░░░  933.2 ms   (1.29× faster) 🏆
          │
n = 1M    │ Dijkstra ████████░░░░░░░░░░░░  2,921.2 ms
          │ BMSSP    ████████████████░░░░░  4,386.5 ms  (1.5× slower)
          │ BktScan  ██████░░░░░░░░░░░░░░  2,176.3 ms  (1.34× faster) 🏆
```

---

## 📉 Heap Operations — All Graph Types

### RandomSparse

<table>
<tr><th>n</th><th>Dijkstra</th><th>BMSSP</th><th>BucketScan</th><th>Best</th></tr>
<tr><td align="right">1,000</td><td align="right">2,198</td><td align="right"><b>319</b> <sub>(6.9×↓)</sub></td><td align="right">1,221 <sub>(1.8×↓)</sub></td><td>🎯 BMSSP</td></tr>
<tr><td align="right">5,000</td><td align="right">11,010</td><td align="right"><b>719</b> <sub>(15.3×↓)</sub></td><td align="right">5,680 <sub>(1.9×↓)</sub></td><td>🎯 BMSSP</td></tr>
<tr><td align="right">10,000</td><td align="right">22,046</td><td align="right"><b>1,369</b> <sub>(16.1×↓)</sub></td><td align="right">11,393 <sub>(1.9×↓)</sub></td><td>🎯 BMSSP</td></tr>
<tr><td align="right">50,000</td><td align="right">110,209</td><td align="right"><b>3,856</b> <sub>(28.6×↓)</sub></td><td align="right">55,549 <sub>(2.0×↓)</sub></td><td>🎯 BMSSP</td></tr>
<tr><td align="right">100,000</td><td align="right">219,882</td><td align="right"><b>7,387</b> <sub>(29.8×↓)</sub></td><td align="right">110,004 <sub>(2.0×↓)</sub></td><td>🎯 BMSSP</td></tr>
<tr><td align="right">500,000</td><td align="right">1,100,457</td><td align="right"><b>22,496</b> <sub>(48.9×↓)</sub></td><td align="right">543,958 <sub>(2.0×↓)</sub></td><td>🎯 BMSSP</td></tr>
<tr><td align="right">1,000,000</td><td align="right">2,200,229</td><td align="right"><b>38,297</b> <sub>(57.4×↓)</sub></td><td align="right">1,087,905 <sub>(2.0×↓)</sub></td><td>🎯 BMSSP</td></tr>
</table>

### Grid

<table>
<tr><th>n</th><th>Dijkstra</th><th>BMSSP</th><th>BucketScan</th><th>Best</th></tr>
<tr><td align="right">10,000</td><td align="right">21,934</td><td align="right"><b>5,179</b> <sub>(4.2×↓)</sub></td><td align="right">11,571 <sub>(1.9×↓)</sub></td><td>🎯 BMSSP</td></tr>
<tr><td align="right">40,000</td><td align="right">87,880</td><td align="right"><b>20,734</b> <sub>(4.2×↓)</sub></td><td align="right">44,974 <sub>(2.0×↓)</sub></td><td>🎯 BMSSP</td></tr>
<tr><td align="right">90,000</td><td align="right">197,791</td><td align="right"><b>32,689</b> <sub>(6.1×↓)</sub></td><td align="right">99,810 <sub>(2.0×↓)</sub></td><td>🎯 BMSSP</td></tr>
<tr><td align="right">250,000</td><td align="right">549,296</td><td align="right"><b>54,348</b> <sub>(10.1×↓)</sub></td><td align="right">275,354 <sub>(2.0×↓)</sub></td><td>🎯 BMSSP</td></tr>
<tr><td align="right">1,000,000</td><td align="right">2,197,802</td><td align="right"><b>218,696</b> <sub>(10.0×↓)</sub></td><td align="right">1,088,134 <sub>(2.0×↓)</sub></td><td>🎯 BMSSP</td></tr>
</table>

---

## ⚖️ The Trade-off Visualized

```
                SPEED                          HEAP OPS
    (higher = faster than Dijkstra)   (higher = fewer than Dijkstra)

    1.5× │         ⚡                   60× │
         │        ╱  BucketScan             │         BMSSP
    1.3× │       ╱                     50× │          🎯
         │      ╱                           │         ╱
    1.1× │ ────╱─ Dijkstra baseline    40× │        ╱
         │   ╱                              │       ╱
    0.9× │  ╱ (Dijkstra = 1.0×)       30× │      ╱
         │ ╱                                │     ╱
    0.7× │╱                            20× │    ╱
         │                                  │   ╱
    0.5× │  BMSSP                      10× │  ╱      ⚡ BucketScan
         │  🎯                              │ ╱ ─────────────────
    0.3× │                              2× │╱  (consistent ~2×)
         └───────────────────────           └───────────────────
         10K  50K 100K 500K  1M             10K  50K 100K 500K  1M
```

### The Sweet Spot

| Want... | Choose... | Why |
|:--------|:----------|:----|
| ⚡ **Fastest possible** | **BucketScan** | 1.0–1.5× faster than Dijkstra at scale |
| 🎯 **Minimal heap ops** | **BMSSP** | Up to 57× fewer (theoretical breakthrough) |
| ⚡🎯 **Both speed AND fewer heap ops** | **BucketScan** | Only algorithm that wins on *both* metrics |
| 🛡️ **Safe, well-known** | **Dijkstra** | 65+ years of battle-tested reliability |

---

## 🔍 Detailed Results by Graph Family

<details>
<summary><b>📊 RandomSparse — Full Results (click to expand)</b></summary>

| n | m | Dijkstra (ms) | BMSSP (ms) | BucketScan (ms) | BucketScan Speedup | BucketScan HeapOps |
|---:|---:|---:|---:|---:|:---|---:|
| 10 | 28 | 0.007 | 0.057 | 0.007 | 1.03× slower | 19 |
| 100 | 296 | 0.074 | 0.537 | 0.083 | 1.12× slower | 145 |
| 1,000 | 2,997 | 1.058 | 3.424 | 1.073 | 1.01× slower | 1,221 |
| 5,000 | 14,996 | 7.820 | 19.995 | 9.441 | 1.21× slower | 5,680 |
| 10,000 | 29,997 | 22.712 | 64.406 | **20.881** | 🏆 1.09× faster | 11,393 |
| 50,000 | 149,998 | 127.929 | 287.191 | **87.140** | 🏆 1.47× faster | 55,549 |
| 100,000 | 299,998 | 164.856 | 339.883 | **163.584** | 🏆 1.01× faster | 110,004 |
| 500,000 | 1,499,997 | 1,202.875 | 1,893.151 | **933.159** | 🏆 1.29× faster | 543,958 |
| 1,000,000 | 2,999,997 | 2,921.155 | 4,386.506 | **2,176.303** | 🏆 1.34× faster | 1,087,905 |

</details>

<details>
<summary><b>🔲 Grid — Full Results (click to expand)</b></summary>

| n | m | Dijkstra (ms) | BMSSP (ms) | BucketScan (ms) | BucketScan Speedup | BucketScan HeapOps |
|---:|---:|---:|---:|---:|:---|---:|
| 100 | 180 | 0.079 | 0.282 | **0.041** | 🏆 1.94× faster | 146 |
| 900 | 1,740 | 0.413 | 1.413 | **0.400** | 🏆 1.03× faster | 1,136 |
| 2,500 | 4,900 | 1.223 | 3.285 | **1.173** | 🏆 1.04× faster | 3,006 |
| 10,000 | 19,800 | 5.586 | 13.308 | **5.130** | 🏆 1.09× faster | 11,571 |
| 40,000 | 79,600 | 29.487 | 55.982 | **25.597** | 🏆 1.15× faster | 44,974 |
| 90,000 | 179,400 | 74.193 | 127.549 | **63.749** | 🏆 1.16× faster | 99,810 |
| 250,000 | 499,000 | 211.525 | 319.556 | **186.481** | 🏆 1.13× faster | 275,354 |
| 1,000,000 | 1,998,000 | 949.061 | 1,458.652 | **837.338** | 🏆 1.13× faster | 1,088,134 |

</details>

<details>
<summary><b>⭐ Star — Full Results (click to expand)</b></summary>

| n | m | Dijkstra (ms) | BMSSP (ms) | BucketScan (ms) | BucketScan Speedup | BucketScan HeapOps |
|---:|---:|---:|---:|---:|:---|---:|
| 10 | 9 | 0.003 | 0.021 | 0.004 | 1.41× slower | 18 |
| 100 | 99 | 0.042 | 0.229 | 0.043 | 1.03× slower | 145 |
| 1,000 | 999 | 0.581 | 1.403 | 0.582 | ~equal | 1,272 |
| 10,000 | 9,999 | 9.254 | 17.652 | **8.564** | 🏆 1.08× faster | 11,721 |

</details>

<details>
<summary><b>🔗 LinearChain — Full Results (click to expand)</b></summary>

| n | m | Dijkstra (ms) | BMSSP (ms) | BucketScan (ms) | BucketScan Speedup | BucketScan HeapOps |
|---:|---:|---:|---:|---:|:---|---:|
| 10 | 9 | 0.002 | 0.060 | 0.014 | 6.5× slower | 20 |
| 100 | 99 | 0.016 | 0.374 | 0.075 | 4.56× slower | 200 |
| 1,000 | 999 | 0.162 | 8.282 | 0.821 | 5.06× slower | 2,000 |
| 5,000 | 4,999 | 0.920 | 49.925 | 2.715 | 2.95× slower | 10,000 |
| 10,000 | 9,999 | 1.583 | 18.567 | 6.266 | 3.96× slower | 20,000 |

> ⚠️ **LinearChain** is BucketScan's weakest graph type — uniform unit weights cause 1 vertex per bucket, negating the bucket advantage. Dijkstra wins here.

</details>

<details>
<summary><b>🕸️ Complete — Full Results (click to expand)</b></summary>

| n | m | Dijkstra (ms) | BMSSP (ms) | BucketScan (ms) | BucketScan Speedup | BucketScan HeapOps |
|---:|---:|---:|---:|---:|:---|---:|
| 10 | 90 | 0.011 | 0.073 | 0.016 | 1.41× slower | 25 |
| 50 | 2,450 | 0.069 | 0.292 | 0.096 | 1.38× slower | 194 |
| 100 | 9,900 | 0.220 | 1.076 | 0.312 | 1.42× slower | 392 |

> Complete graphs have small n in our tests. BucketScan's overhead isn't amortized at these sizes.

</details>

---

## 📋 Scorecard Summary

<table>
<tr>
<th>Graph Family</th>
<th>BucketScan Wins Speed?</th>
<th>BucketScan Wins Heap?</th>
<th>Best Graph Size</th>
</tr>
<tr>
<td><b>RandomSparse</b></td>
<td>✅ Yes (n ≥ 10K)</td>
<td>✅ Yes (all sizes)</td>
<td>n = 50K → <b>1.47×</b> faster</td>
</tr>
<tr>
<td><b>Grid</b></td>
<td>✅ Yes (all sizes!)</td>
<td>✅ Yes (all sizes)</td>
<td>n = 100 → <b>1.94×</b> faster</td>
</tr>
<tr>
<td><b>Star</b></td>
<td>✅ Yes (n ≥ 10K)</td>
<td>✅ Yes (all sizes)</td>
<td>n = 10K → <b>1.08×</b> faster</td>
</tr>
<tr>
<td><b>LinearChain</b></td>
<td>❌ No (Dijkstra wins)</td>
<td>⚖️ Equal</td>
<td>— (unit weights, degenerate case)</td>
</tr>
<tr>
<td><b>Complete</b></td>
<td>❌ No (small n)</td>
<td>✅ Yes</td>
<td>— (only tested n ≤ 100)</td>
</tr>
</table>

---

## ⚙️ Benchmark Environment

| Parameter | Value |
|:----------|:------|
| **Runtime** | .NET 10.0.5 |
| **OS** | Microsoft Windows NT 10.0.26200.0 |
| **Machine** | LAPTOP-IVNV7JPF |
| **CPU Cores** | 16 logical cores |
| **Warm-up** | Yes (for n ≤ 1,000) |
| **Correctness tolerance** | 1e-9 (floating-point) |
| **Full data** | [`results/benchmark-results.md`](../results/benchmark-results.md) |

---

<p align="center">
  <sub>📊 Dashboard generated from actual benchmark data • See <a href="BUCKETSCAN.md">BucketScan algorithm docs</a> for technical details</sub>
</p>
